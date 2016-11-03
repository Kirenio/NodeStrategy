using UnityEngine;

public delegate void CargoCartEventHandler();

public class CargoCart : MonoBehaviour {
    float CurrentHealth;
    public float MaxHealth;
    public float Capacity;
    public float Speed;
    public bool Paused = false;
    bool waitingCargo = false;
    public bool ReturningCargo = false;
    public bool ResourceChangePending { get; protected set; }

    public Resource ResourceToShip;
    public Resource ResourceToShipNew { get; protected set; }
    public float StoredAmount;

    public Building Shipping;
    public string ShippingName { get { if (Shipping == null) return "N/A"; else return Shipping.name; } }
    public Building Recieving;
    public string RecievingName { get { if (Recieving == null) return "N/A"; else return Recieving.name; } }

    public CargoCartEventHandler InventoryChanged;
    public CargoCartEventHandler InventoryEmpty;
    public CargoCartEventHandler ResourceToShipChanged;
    public CargoCartEventHandler PauseEbabled;

    void OnInventoryChanged()
    {
        if(InventoryChanged != null) InventoryChanged();
    }

    protected void Awake()
    {
        CurrentHealth = MaxHealth;
        ResourceChangePending = false;
        if (Shipping == null || Recieving == null) Paused = true;
    }

    void Update()
    {
        if (Paused) return;

        if (ReturningCargo)
        {
            transform.LookAt(Shipping.PortPos);
            transform.position = Vector3.MoveTowards(transform.position, Shipping.PortPos, Speed * Time.deltaTime);
            if (transform.position == Shipping.PortPos)
            {
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
        Debug.Log("Queueing a new resource to ship!");
        ResourceChangePending = true;
        ResourceToShipNew = value;
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
