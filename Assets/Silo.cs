using UnityEngine;
using System.Collections.Generic;

public class Silo : Building
{
    protected override void Awake()
    {
        base.Awake();
    }
    public override Cargo Recieve(Cargo cargo)
    {
        if (Stored.ContainsKey(cargo.Type) && Stored[cargo.Type] == StoredAmount)
        {
            if (StoredAmount + cargo.Amount <= Capacity)
            {
                Stored[cargo.Type] = Stored[cargo.Type] + cargo.Amount;
                StoredAmount += cargo.Amount;
                LogRecieved(cargo);
                return CargoCreate(cargo.Type, 0);
            }
            else
            {
                float spaceFree = Capacity - StoredAmount;

                Stored[cargo.Type] = Stored[cargo.Type] + spaceFree;
                StoredAmount += spaceFree;
                LogRecieved(cargo.Type, spaceFree);
                return CargoCreate(cargo.Type, cargo.Amount - spaceFree);
            }
        }
        else if (StoredAmount == 0)
        {
            Stored.Add(cargo.Type, cargo.Amount);
            StoredAmount += cargo.Amount;
            LogRecieved(cargo);
            return CargoCreate(cargo.Type, 0);
        }
        return cargo;
    }
}
