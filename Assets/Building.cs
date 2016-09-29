using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
    public float CurrentHealth;
    public float MaxHealth;
    public float Capacity;
    public Cargo Stored;
    
    public Transform PortPos;

    protected virtual void Awake()
    {
        PortPos = transform.GetChild(0).transform;
    }
    
    public virtual Cargo CargoCreate(Resource type, float amount)
    {
        return new Cargo(type, amount);
    }
    public virtual Cargo Ship (Resource type, float amount)
    {
        if (Stored.Type == type)
        {
            if (amount > Stored.Amount)
            {
                float rValue = Stored.Amount;
                Stored.Amount = 0;
                return CargoCreate(Stored.Type, rValue);
            }
            else
            {
                Stored.Amount -= amount;
                return CargoCreate(Stored.Type, amount);
            }
        }
        return CargoCreate(Resource.Empty, 0);
    }

    public virtual Cargo Recieve(Cargo cargo)
    {
        if(Stored.Type == Resource.Empty || Stored.Amount == 0)
        {
            Stored = cargo;
            return CargoCreate(cargo.Type, 0);
        }
        else if (Stored.Type == cargo.Type)
        {
            if (Stored.Amount + cargo.Amount > Capacity)
            {
                Stored.Amount = Capacity;
                return CargoCreate(cargo.Type, cargo.Amount + Stored.Amount - Capacity);
            }
            else
            {
                Stored.Amount += cargo.Amount;
                return CargoCreate(cargo.Type, 0);
            }
        }
        else return cargo;
    }
}