using UnityEngine;
using System.Collections;

public class Manufactory : Building
{
    public Resource Requirements;
    public Resource Production;
    public float AmountRequired;
    public float AmountProduced;
    public int ProductCapacity;

    protected override void Awake()
    {
        base.Awake();
        Stored.Add(Production, 0);
    }
    protected virtual void Update () {
        if (Stored.ContainsKey(Requirements))
        {
            if (AmountRequired <= Stored[Requirements] && Stored[Production] < ProductCapacity)
            {
                Produce();
            }
        }
	}
    protected virtual void LogProduced()
    {
        if(LogActivity)
        Debug.Log(string.Format("{0}: Produced {1} {2}\nUsed {3} out of {4} space.",
            gameObject.name, AmountProduced, Production, Stored[Production], ProductCapacity));
    }
    protected virtual void Produce()
    {
        if(Stored.ContainsKey(Production))
        {
            Stored[Requirements] -= AmountRequired;
            Stored[Production] += AmountProduced;
            StoredAmount -= AmountRequired;
        }
        else
        {
            Stored[Requirements] -= AmountRequired;
            Stored.Add(Production, AmountProduced);
            StoredAmount -= AmountRequired;

        }
        LogProduced();
    }
}
