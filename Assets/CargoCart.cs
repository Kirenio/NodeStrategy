using System.Collections.Generic;
using UnityEngine;

public delegate void CargoCartEventHandler();
public delegate void CargoCartTargetingEventHandler(Building building);

public class CargoCart : MonoBehaviour {
    float CurrentHealth;
    public float MaxHealth;
    public float Capacity;
    public float Speed;
    public bool Paused = false;
    public bool waitingCargo = false;
    public bool ReturningCargo = false;
    public bool ResourceChangePending { get; protected set; }

    public Resource ResourceToShip;
    public Resource ResourceToShipNew { get; protected set; }
    public float StoredAmount;

    public Building Shipping;
    public string ShippingName { get { if (Shipping == null) return "N/A"; else return Shipping.name; } }
    bool recievingIsAConstruction = false; // (!) Implement a better system to determine what are we delivering to
    protected Building recieving;
    public Building Recieving { get { return recieving; } set { recieving = value; if (NewRecieving != null) NewRecieving(value); } }
    public string RecievingName { get { if (Recieving == null) return "N/A"; else return Recieving.name; } }

    public CargoCartEventHandler InventoryChanged;
    public CargoCartEventHandler InventoryEmpty;
    public CargoCartEventHandler ResourceChangeQueued;
    public CargoCartEventHandler ResourceToShipChanged;
    public CargoCartEventHandler PauseEbabled;
    CargoCartTargetingEventHandler NewRecieving;

    void OnInventoryChanged()
    {
        if(InventoryChanged != null) InventoryChanged();
    }

    protected void Awake()
    {
        CurrentHealth = MaxHealth;
        NewRecieving += ResolveBehaviour;
        ResourceChangePending = false;
        if (Shipping == null || Recieving == null) Paused = true;
    }

    void Update()
    {
        if (Paused) return;

        if (ReturningCargo)
        {
            ReturnCargo();
        }

        if (!waitingCargo)
        {
            if (StoredAmount == 0)
            {
                MoveToShipping();
            }
            else
            {
                MoveToRecieving();
            }
        }
    }

    //  Behaviour functions

    void ResolveBehaviour(Building building)
    {
        waitingCargo = false;
        recievingIsAConstruction = false;
        if (building.GetType() == typeof(ConstructionSite))
        {
            recievingIsAConstruction = true;

            foreach (KeyValuePair<Resource, float> entry in building.Stored)
            {
                Debug.Log("Recieved Key is: " + entry.Key);

                if (entry.Value - Recieving.ResourcesIncoming[entry.Key] > 0)
                {
                    QueueResourceChange(entry.Key);
                    break;
                }
            }

            Paused = false;
            PauseEbabled();
        }
        else if(building == null)
        {
            ReturningCargo = true;
        }
        else
            recievingIsAConstruction = false;
    }

    void ReturnCargo()
    {
        Debug.Log("ReturnCargo");
        transform.LookAt(Shipping.PortPos);
        transform.position = Vector3.MoveTowards(transform.position, Shipping.PortPos, Speed * Time.deltaTime);
        if (transform.position == Shipping.PortPos)
        {
            // Getting enough resources
            StoredAmount = Shipping.Recieve(CargoCreate(ResourceToShip, StoredAmount));
            if (StoredAmount == 0)
            {
                ReturningCargo = false;
                Paused = true;
                if (InventoryEmpty != null) InventoryEmpty();
                if (PauseEbabled != null) PauseEbabled();
            }
            OnInventoryChanged();
        }
        return;
    }

    void GetNewResourceToShip()
    {
        waitingCargo = true;
        foreach (KeyValuePair<Resource, float> entry in Recieving.Stored)
        {
            if (entry.Value - Recieving.ResourcesIncoming[entry.Key] > 0)
            {
                Debug.Log("Recieved Key is: " + entry.Key);
                QueueResourceChange(entry.Key);
                waitingCargo = false;
                Recieving.InventoryChanged -= GetNewResourceToShip;
                break;
            }
        }
        Recieving.InventoryChanged += GetNewResourceToShip;
    }



    void MoveToShipping()
    {
        if (InventoryEmpty != null) InventoryEmpty();
        
        transform.LookAt(Shipping.PortPos);
        transform.position = Vector3.MoveTowards(transform.position, Shipping.PortPos, Speed * Time.deltaTime);
        if (transform.position == Shipping.PortPos)
        {
            float quota = Recieving.Quota(ResourceToShip, StoredAmount);
            if(quota == 0)
            {
                Debug.Log("Quota was less than 0");
                if (recievingIsAConstruction)
                {
                    GetNewResourceToShip();
                }
                else
                {
                    waitingCargo = true;
                    Recieving.InventoryChanged += StopWatingCargo;
                }
            }

            if (ResourceChangePending) return;

            Debug.Log("Quota is " + quota);

            if (quota >= Capacity)
            {
                StoredAmount = Shipping.Ship(ResourceToShip, Capacity);
                Recieving.RegisterShipment(ResourceToShip, StoredAmount);
            }
            else if (quota < Capacity && quota > 0)
            {
                StoredAmount = Shipping.Ship(ResourceToShip, Recieving.Quota(ResourceToShip, StoredAmount));
                Recieving.RegisterShipment(ResourceToShip, StoredAmount);
            }

            if(StoredAmount == 0)
            {
                waitingCargo = true;
                Shipping.InventoryChanged += StopWatingCargo;
            }

            OnInventoryChanged();
        }
    }

    void MoveToRecieving()
    {
        //Debug.Log("MoveToRecieving");
        transform.LookAt(Recieving.PortPos);
        transform.position = Vector3.MoveTowards(transform.position, Recieving.PortPos, Speed * Time.deltaTime);

        if (transform.position == Recieving.PortPos)
        {
            float left = Recieving.Recieve(CargoCreate(ResourceToShip, StoredAmount));
            Recieving.UpdateShipment(ResourceToShip, StoredAmount - left);
            StoredAmount = left;

            if (StoredAmount != 0)
            {
                waitingCargo = true;
                Recieving.InventoryChanged += StopWatingCargo;
            }

            OnInventoryChanged();
        }
    }

    // Utility functions

    public virtual Cargo CargoCreate(Resource type, float amount)
    {
        return new Cargo(type, amount);
    }

    void StopWatingCargo()
    {
        waitingCargo = false;
        Recieving.InventoryChanged -= StopWatingCargo;
    }

    public void QueueResourceChange(Resource value)
    {
        Debug.Log("Queueing a new resource to ship!");
        ResourceChangePending = true;
        ResourceToShipNew = value;
        if (ResourceChangeQueued != null) ResourceChangeQueued();
        InventoryEmpty += ChangeResource;
    }

    void ChangeResource()
    {
        if(StoredAmount == 0)
        {
            Debug.Log("Resource changed successfully!");
            ResourceToShip = ResourceToShipNew;
            ResourceChangePending = false;
            if (ResourceToShipChanged != null) ResourceToShipChanged();
            InventoryEmpty -= ChangeResource;
            waitingCargo = false;
        }
    }

    public virtual void DumpCargo()
    {
        StoredAmount = 0;
        OnInventoryChanged();
    }

    public virtual string GetContentString()
    {
        if (StoredAmount > 0)
        {
            return string.Format("Stored materials\n{0} {1}", StoredAmount, ResourceToShip);
        }
        else return "Stored materials\n";
    }

    public virtual string GetStatsString()
    {
        return string.Format("HP:\t\t\t{0}/{1}\nStorage:\t{2}/{3}\nSpeed:\t\t{4}", CurrentHealth, MaxHealth, StoredAmount, Capacity, Speed);
    }
}
