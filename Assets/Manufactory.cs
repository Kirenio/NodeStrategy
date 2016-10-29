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
        InventoryChangedInternal += Produce;
    }

    protected virtual void Produce()
    {
        if (Stored.ContainsKey(Requirements))
        {
            if (AmountRequired <= Stored[Requirements])
            {
                Stored[Requirements] -= AmountRequired;

                if (Stored.ContainsKey(Production))
                    Stored[Production] += AmountProduced;
                else
                    Stored.Add(Production, AmountProduced);

                StoredAmount += AmountProduced - AmountRequired;

                OnInventoryChanged();
                if (LogActivity) LogProduced();
            }
        }
    }

    protected virtual void LogProduced()
    {
        Debug.Log(string.Format("{0}: Produced {1} {2}\nUsed {3} out of {4} space.",
            gameObject.name, AmountProduced, Production, StoredAmount, Capacity));
    }

    public override string GetStatsString()
    {
        return string.Format("HP:\t{0}/{1}\nStorage:\t{2}/{3}\nProduction\n{4} {5}\nRequirements\n{6} {7}", CurrentHealth, MaxHealth, StoredAmount, Capacity, AmountProduced, Production, AmountRequired, Requirements);
    }
}
