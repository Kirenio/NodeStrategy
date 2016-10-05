using UnityEngine;

public class Extractor : Building {
    public Resource producedResource;
    public float extractionRate;

    protected override void Awake()
    {
        base.Awake();
        Recieve(CargoCreate(producedResource, 0));
    }

    void LogExtracted(float amount)
    {
        Debug.Log(string.Format("{0}: Extracted {1} {2}\n Used {3} out of {4} space.",gameObject.name, amount, producedResource, StoredAmount, Capacity));
    }
    void Update()
    {
        if(Stored.ContainsKey(producedResource))
        {
            float extractedAmount = extractionRate * Time.deltaTime;
            if (Stored[producedResource] + extractedAmount < Capacity)
            {
                Stored[producedResource] += extractedAmount;
                StoredAmount += extractedAmount;
                if (LogActivity) LogExtracted(extractedAmount);
            }
            else
            {
                if (LogActivity) LogExtracted(Capacity - StoredAmount);
                Stored[producedResource] = Capacity;
                StoredAmount = Capacity;
            }
        }
    }
}
