using UnityEngine;
using System.Collections.Generic;

public class Warehouse : Building {
    public new Dictionary<Resource, Cargo> Stored = new Dictionary<Resource, Cargo>();
    
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
        if (Stored.ContainsKey(cargo.Type))
        {
            Stored[cargo.Type] = CargoCreate(cargo.Type, Stored[cargo.Type].Amount + cargo.Amount);
            Debug.Log(Stored[cargo.Type].Amount + " " + Stored[cargo.Type].Type);
            return CargoCreate(cargo.Type, 0);
        }
        else if (!Stored.ContainsKey(cargo.Type))
        {
            Stored.Add(cargo.Type, cargo);
            Debug.Log(Stored[cargo.Type].Amount + " " + Stored[cargo.Type].Type);
            return CargoCreate(cargo.Type, 0);
        }
        else return cargo;
    }
}