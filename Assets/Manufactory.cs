using UnityEngine;
using System.Collections;

public class Manufactory : Building
{
    public Resource Requirements;
    public Resource Production;
    public int AmountRequired;
    public Cargo ProductStored;
    public int ProductCapacity;

    void Update () {
	    if(AmountRequired <= Stored.Amount && ProductStored.Amount < ProductCapacity)
        {
            ProductStored.Amount++;
            Stored.Amount -= AmountRequired;
        }
	}
}
