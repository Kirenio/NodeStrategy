using UnityEngine;
using System.Collections.Generic;

public delegate void BuildingEventHandler();

public class Building : MonoBehaviour
{
    public float CurrentHealth;
    public float MaxHealth;
    public Dictionary<Resource, float> Stored = new Dictionary<Resource, float>();
    public float StoredAmount = 0;
    public float Capacity;

    public Vector3 PortPos;

    public bool LogActivity = false;

    public event BuildingEventHandler InventoryChanged;
    protected event BuildingEventHandler InventoryChangedInternal;

    protected virtual void OnInventoryChanged()
    {
        if (InventoryChanged != null) InventoryChanged();
    }

    protected virtual void OnInventoryChangedInternal()
    {
        if (InventoryChangedInternal != null) InventoryChangedInternal();
    }

    protected virtual void Awake()
    {
        PortPos = transform.GetChild(0).transform.position;
        PortPos = new Vector3(PortPos.x, 0.00f, PortPos.z);
        Destroy(transform.GetChild(0).gameObject);
        CurrentHealth = MaxHealth;
    }
    
    public virtual Cargo CargoCreate(Resource type, float amount)
    {
        return new Cargo(type, amount);
    }

    public virtual float Ship (Resource type, float amount)
    {
        if (Stored.ContainsKey(type))
        {
            if (amount > Stored[type])
            {
                float rValue = Stored[type];
                StoredAmount -= Stored[type];
                Stored[type] = 0;
                if (InventoryChanged != null) InventoryChanged();
                return rValue;
            }
            else
            {
                Stored[type] -= amount;
                StoredAmount -= amount;
                if (InventoryChanged != null) InventoryChanged();
                return amount;
            }
        }
        else return 0;
    }

    public virtual float Recieve(Cargo cargo)
    {
        if (StoredAmount + cargo.Amount <= Capacity)
        {
            if (Stored.ContainsKey(cargo.Type))
                Stored[cargo.Type] = Stored[cargo.Type] + cargo.Amount;
            else
                Stored.Add(cargo.Type, cargo.Amount);

                StoredAmount += cargo.Amount;

                if (LogActivity) LogRecieved(cargo);

            if (InventoryChanged != null) InventoryChanged();
            if (InventoryChangedInternal != null) InventoryChangedInternal();
                return 0;
        }
        else if (StoredAmount + cargo.Amount > Capacity)
        {
            float spaceFree = Capacity - StoredAmount;
            if (spaceFree > 0)
            {
                if (Stored.ContainsKey(cargo.Type))
                    Stored[cargo.Type] = Stored[cargo.Type] + spaceFree;
                else
                    Stored.Add(cargo.Type, spaceFree);

                StoredAmount += spaceFree;

                if (LogActivity) LogRecieved(cargo.Type, spaceFree);

                if (InventoryChanged != null) InventoryChanged();
                if (InventoryChangedInternal != null) InventoryChangedInternal();
                return cargo.Amount - spaceFree;
            }
        }
        return cargo.Amount;
    }
    
    protected virtual void LogRecieved(Cargo cargo)
    {
        Debug.Log(string.Format("{0}: {1} {2}\nUsed {3} out of {4} space.",
            gameObject.name, Stored[cargo.Type], cargo.Type, StoredAmount, Capacity));
    }

    protected virtual void LogRecieved(Resource type, float amount)
    {
        Debug.Log(string.Format("{0}: {1} {2}\nUsed {3} out of {4} space.",
            gameObject.name, Stored[type], amount, StoredAmount, Capacity));
    }

    public virtual void LogContent()
    {
        string content;
        content = string.Format("{0}: Used {1} out of {2} space. ", gameObject.name, StoredAmount, Capacity);
        foreach (KeyValuePair<Resource, float> entry in Stored)
        {
            if (entry.Value > 0)
                content += string.Format("{0} {1}\n", entry.Value, entry.Key);
        }
        Debug.Log(content);
    }

    public virtual string GetContentString()
    {
        string content = "Stored materials\n";
        foreach(KeyValuePair<Resource, float> entry in Stored)
        {
            if (entry.Value > 0)
                content += string.Format("{0} {1}\n", entry.Value, entry.Key);
        }
        return content;
    }

    public virtual string GetStatsString()
    {
        return string.Format("HP:\t{0}/{1}\nStorage:\t{2}/{3}", CurrentHealth, MaxHealth, StoredAmount, Capacity);
    }
}