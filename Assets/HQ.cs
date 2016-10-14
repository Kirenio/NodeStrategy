using UnityEngine;
using System.Collections;

public class HQ : Building {

    protected override void Awake()
    {
        base.Awake();
        Recieve(CargoCreate(Resource.Composites, 200));
        Recieve(CargoCreate(Resource.Electronics, 50));
    }
}
