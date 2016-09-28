using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
    public float CurrentHealth;// { get; protected set; }
    public float MaxHealth;// { get; protected set; }
    public float Capacity;// { get; protected set; }
    public float Stored;// { get; protected set; }
    
    public Transform PortPos;

    void Awake()
    {
        PortPos = transform.GetChild(0).transform;
    }

    // Update is called once per frame
    public virtual float Ship (float amount)
    {
        if (amount > Stored)
        {
            float rAmount = Stored;
            Stored = 0;
            return rAmount;
        }
        else
        {
            Stored -= amount;
            return amount;
        }
    }

    public virtual float Recieve(float amount)
    {
        if (Stored + amount > Capacity)
        {
            Stored = Capacity;
            return Stored + amount - Capacity;
        }
        else
        {
            Stored += amount;
            return 0;
        }
    }
}
