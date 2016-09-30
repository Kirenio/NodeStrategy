using UnityEngine;
using System.Collections;

public class Extractor : Building {
    public Resource producedResource;
    public float extractionRate;

    protected override void Awake()
    {
        base.Awake();
        Recieve(CargoCreate(producedResource, 0));
    }

    void Update()
    {
        if(Stored.ContainsKey(producedResource))
        {
            float extractedAmount = extractionRate * Time.deltaTime;
            if (Stored[producedResource] + extractedAmount < Capacity)
            {
                Stored[producedResource] += extractedAmount;
            }
            else
            {
                Stored[producedResource] = Capacity;
            }
        }
    }
}
