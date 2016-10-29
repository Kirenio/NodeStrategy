using UnityEngine;

public delegate void CargoCartEventHandler();

public class CargoCart : MonoBehaviour {
    float CurrentHealth;
    public float MaxHealth;
    public float Capacity;
    public float Speed;
    public bool Paused = false;
    bool waitingCargo = false;
    public bool ResourceChangePending = false;

    public Resource ResourceToShip;
    public Resource ResourceToShipNew { get; private set; }
    public float StoredAmount;

    public Building Shipping;
    public Building Recieving;

    public CargoCartEventHandler InventoryChanged;
    public CargoCartEventHandler InventoryEmpty;

    void OnInventoryChanged()
    {
        if(InventoryChanged != null) InventoryChanged();
    }

    protected void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    void Update()
    {
        if (Paused) return;

        if (StoredAmount == 0)
        {
            if (InventoryEmpty != null) InventoryEmpty();
            transform.LookAt(Shipping.PortPos);
            transform.position = Vector3.MoveTowards(transform.position, Shipping.PortPos, Speed * Time.deltaTime);
            if (transform.position == Shipping.PortPos)
            {
                StoredAmount = Shipping.Ship(ResourceToShip, Capacity);
                OnInventoryChanged();
            }
        }
        else
        {
            if (!waitingCargo)
            {
                transform.LookAt(Recieving.PortPos);
                transform.position = Vector3.MoveTowards(transform.position, Recieving.PortPos, Speed * Time.deltaTime);
                if (transform.position == Recieving.PortPos)
                {
                    StoredAmount = Recieving.Recieve(CargoCreate(ResourceToShip, StoredAmount));
                    if (StoredAmount != 0)
                    {
                        waitingCargo = true;
                        Recieving.InventoryChanged += StopWatingCargo;
                    }
                    OnInventoryChanged();
                }
            }
        }
    }

    public virtual Cargo CargoCreate(Resource type, float amount)
    {
        return new Cargo(type, amount);
    }

    void StopWatingCargo()
    {
        waitingCargo = true;
        Recieving.InventoryChanged -= StopWatingCargo;
    }

    public void QueueResourceChange(Resource value)
    {
        ResourceChangePending = true;
        ResourceToShipNew = value;
        InventoryEmpty += ChangeResource;
    }

    void ChangeResource()
    {
        if(StoredAmount == 0)
        {
            ResourceToShip = ResourceToShipNew;
            ResourceChangePending = false;
            InventoryEmpty -= ChangeResource;
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
