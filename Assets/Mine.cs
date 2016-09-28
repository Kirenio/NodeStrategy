using UnityEngine;
using System.Collections;

public class Mine : Building {
    public float extractionRate;

    void Update()
    {
        float extractedAmount = extractionRate * Time.deltaTime;
        if (Stored + extractedAmount < Capacity)
        {
            Stored += extractedAmount;
        }
        else
        {
            Stored = Capacity;
        }
    }

    //public override float Ship(float amount)
    //{
    //    if(amount > Stored)
    //    {
    //        float rAmount = Stored;
    //        Stored = 0;
    //        return rAmount;
    //    }
    //    else
    //    {
    //        Stored -= amount;
    //        return amount;
    //    }
    //}
}
