﻿using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour {
    public float CurrentHealth;
    public float MaxHealth;
    public Dictionary<Resource, float> Stored = new Dictionary<Resource, float>();
    public float StoredAmount = 0;
    public float Capacity;

    public Transform PortPos;

    public bool LogActivity = false;

    protected virtual void Awake()
    {
        PortPos = transform.GetChild(0).transform;
        CurrentHealth = MaxHealth;
    }
    
    public virtual Cargo CargoCreate(Resource type, float amount)
    {
        return new Cargo(type, amount);
    }

    public virtual Cargo Ship (Resource type, float amount)
    {
        if (Stored.ContainsKey(type))
        {
            if (amount > Stored[type])
            {
                Cargo rValue = CargoCreate(type, Stored[type]);
                StoredAmount -= Stored[type];
                Stored[type] = 0;
                return rValue;
            }
            else
            {
                Stored[type] -= amount;
                StoredAmount -= amount;
                return CargoCreate(type, amount);
            }
        }
        else return CargoCreate(Resource.Empty, 0);
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
        content = string.Format("{0}: Used {1} out of {2} space. ", gameObject.name, StoredAmount,Capacity);
        foreach (KeyValuePair<Resource,float> entry in Stored)
        {
            content += string.Format("{0} {1}\n",entry.Value, entry.Key);
        }
        Debug.Log(content);
    }

    public virtual Cargo Recieve(Cargo cargo)
    {
        if (StoredAmount + cargo.Amount <= Capacity)
        {
            if (Stored.ContainsKey(cargo.Type))
                Stored[cargo.Type] = Stored[cargo.Type] + cargo.Amount;
            else
                Stored.Add(cargo.Type, cargo.Amount);

                StoredAmount += cargo.Amount;

                if (LogActivity) LogRecieved(cargo);

                return CargoCreate(cargo.Type, 0);
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

                return CargoCreate(cargo.Type, cargo.Amount - spaceFree);
            }
        }
        return cargo;
    }
}