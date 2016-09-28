using UnityEngine;
using System.Collections;

public class Extractor : Building {
    public Resource producedResource;
    public float extractionRate;

    protected override void Awake()
    {
        base.Awake();
        Stored = CargoCreate(producedResource, 0);
    }

    void Update()
    {
        if(Stored.Type == producedResource)
        {
            float extractedAmount = extractionRate * Time.deltaTime;
            if (Stored.Amount + extractedAmount < Capacity)
            {
                Stored.Amount += extractedAmount;
            }
            else
            {
                Stored.Amount = Capacity;
            }
        }
    }
}
