using UnityEngine;
using System.Collections;

public enum Resources { Crystal }
public enum Goods { DataCrystal }

public class Manufactory : Building
{
    public Resources Requirements;
    public Goods Production;
    public int AmountRequired;
    public int ProductStored;
    public int ProductCapacity;

    void Update () {
	    if(AmountRequired < Stored && ProductStored < ProductCapacity)
        {
            ProductStored++;
            Stored -= AmountRequired;
        }
	}
}
