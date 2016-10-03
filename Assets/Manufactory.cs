using UnityEngine;

public class Manufactory : Building
{
    public Resource Requirements;
    public Resource Production;
    public float AmountRequired;
    public float AmountProduced;

    protected override void Awake()
    {
        base.Awake();
        Stored.Add(Production, 0);
    }
    protected virtual void Update () {
        if (Stored.ContainsKey(Requirements))
        {
            if (AmountRequired <= Stored[Requirements])
            {
                Produce();
            }
        }
	}
    
    protected virtual void LogProduced()
    {
        Debug.Log(string.Format("{0}: Produced {1} {2}\nUsed {3} out of {4} space.",
            gameObject.name, AmountProduced, Production, StoredAmount, Capacity));
    }
    protected virtual void Produce()
    {
        Stored[Requirements] -= AmountRequired;

        if (Stored.ContainsKey(Production))
            Stored[Production] += AmountProduced;
        else
            Stored.Add(Production, AmountProduced);

        StoredAmount += AmountProduced - AmountRequired;

        if (LogActivity) LogProduced();
    }
}
