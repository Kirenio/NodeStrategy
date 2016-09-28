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
    public virtual Cargo Ship (float amount)
    {
        if (amount > Stored.Amount)
        {
            return CargoCreate(Stored.Type, Stored.Amount);
        }
        else
        {
            Stored.Amount -= amount;
            return CargoCreate(Stored.Type, amount);
        }
    }

    public virtual Cargo Recieve(Cargo cargo)
    {
        if(Stored.Type == Resource.Empty || Stored.Amount == 0)
        {
            Stored = cargo;
        }
        if (Stored.Type == cargo.Type)
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
