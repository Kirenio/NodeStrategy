using UnityEngine;
using System.Collections.Generic;

public class Warehouse : Building {
    public new Dictionary<Resource, Cargo> Stored = new Dictionary<Resource, Cargo>();

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.T))
        {
            Recieve(CargoCreate(Resource.DataCrystal, 1));
        }
        if(Input.GetKeyUp(KeyCode.Y))
        {
            Recieve(CargoCreate(Resource.DataCrystal, 1000000));
        }
    }
    protected override void LogRecieved(Cargo cargo)
    {
        Debug.Log(string.Format("{0}: {1} {2}\nUsed {3} out of {4} space.", gameObject.name, Stored[cargo.Type].Amount, cargo.Type, StoredAmount, Capacity));
    }
    public override Cargo Ship(Resource type, float amount)
    {
        if (Stored.ContainsKey(type))
        {
            if (amount > Stored[type].Amount)
            {
                float rValue = Stored[type].Amount;
                Stored[type] = CargoCreate(type, 0);
                return CargoCreate(type, rValue);
            }
            else
            {
                Stored[type] = CargoCreate(type, Stored[type].Amount - amount);
                return CargoCreate(type, amount);
            }
        }
        else return CargoCreate(type, 0);
    }

    public override Cargo Recieve(Cargo cargo)
    {
        if (StoredAmount + cargo.Amount <= Capacity)
        {
            if (Stored.ContainsKey(cargo.Type))
            {
                Stored[cargo.Type] = CargoCreate(cargo.Type, Stored[cargo.Type].Amount + cargo.Amount);
                StoredAmount += cargo.Amount;
                LogRecieved(cargo);
                return CargoCreate(cargo.Type, 0);
            }
            else
            {
                Stored.Add(cargo.Type, cargo);
                StoredAmount += cargo.Amount;
                LogRecieved(cargo);
                return CargoCreate(cargo.Type, 0);
            }
        }
        else if (StoredAmount + cargo.Amount > Capacity)
        {
            float spaceFree = Capacity - StoredAmount;
            if (spaceFree > 0)
            {
                if (Stored.ContainsKey(cargo.Type))
                {
                    Stored[cargo.Type] = CargoCreate(cargo.Type, Stored[cargo.Type].Amount + spaceFree);
                    StoredAmount += spaceFree;
                    LogRecieved(cargo);
                    return CargoCreate(cargo.Type, cargo.Amount - spaceFree);
                }
                else
                {
                    Stored.Add(cargo.Type, CargoCreate(cargo.Type, spaceFree));
                    StoredAmount += spaceFree;
                    LogRecieved(cargo);
                    return CargoCreate(cargo.Type, cargo.Amount - spaceFree);
                }
            }
        }
        Debug.Log(string.Format("{0}: Cargo Refused - {1} {2}. Storage {3}/{4}",gameObject.name, cargo.Amount, cargo.Type, StoredAmount, Capacity));
        return cargo;
    }

    public virtual float StoredAmountRecalculate()
    {
        float value = 0;
        foreach (Cargo item in Stored.Values)
        {
            value += item.Amount;
        }
        return value;
    }
}