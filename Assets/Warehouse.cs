using UnityEngine;
using System.Collections.Generic;

public class Warehouse : Building {
    public new Dictionary<Resource, Cargo> Stored = new Dictionary<Resource, Cargo>();
    float StoredAmount = 0;
    
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

    protected virtual void LogRecieved(Cargo cargo)
    {
        Debug.Log(string.Format("{0} {1}\nUsed {2} out of {3} space.", Stored[cargo.Type].Amount, Stored[cargo.Type].Type, StoredAmount, Capacity));
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
        else if(StoredAmount + cargo.Amount > Capacity)
        {
            float spaceFree = Capacity - StoredAmount;
            if(spaceFree > 0)
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