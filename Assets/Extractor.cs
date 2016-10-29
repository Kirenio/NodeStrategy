using UnityEngine;

public class Extractor : Building {
    public Resource producedResource;
    public float extractionRate;
    public float extractionTime;
    bool storageFull = false;
    float nextExtraction;

    protected override void Awake()
    {
        base.Awake();
        Recieve(CargoCreate(producedResource, 0));
    }

    void Start()
    {
        nextExtraction = Time.time + extractionTime;
    }

    void LogExtracted(float amount)
    {
        Debug.Log(string.Format("{0}: Extracted {1} {2}\n Used {3} out of {4} space.",gameObject.name, amount, producedResource, StoredAmount, Capacity));
    }
    void Update()
    {
        if (!storageFull && Time.time > nextExtraction)
        {
            if (Stored.ContainsKey(producedResource))
            {
                if (Stored[producedResource] + extractionRate < Capacity)
                {
                    Stored[producedResource] += extractionRate;
                    StoredAmount += extractionRate;
                    if (LogActivity) LogExtracted(extractionRate);
                    OnInventoryChanged();
                }
                else
                {
                    if (LogActivity) LogExtracted(Capacity - StoredAmount);
                    Stored[producedResource] = Capacity;
                    StoredAmount = Capacity;
                    storageFull = true;
                    OnInventoryChanged();
                }
            }
            nextExtraction += extractionTime;
        }
    }

    public override float Ship(Resource type, float amount)
    {
        nextExtraction = Time.time + extractionTime;
        storageFull = false;
        return base.Ship(type, amount);
    }

    public override string GetStatsString()
    {
        return string.Format("HP:\t{0}/{1}\nStorage:\t{2}/{3}\nExtraction\nRate:\t{4}\nTime\t{5}s", CurrentHealth, MaxHealth, StoredAmount, Capacity, extractionRate, extractionTime);
    }
}
