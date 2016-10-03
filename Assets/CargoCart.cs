﻿using UnityEngine;

public class CargoCart : Building {
    public float Speed;

    public Resource ResourceToShip;

    public Building Shipping;
    public Building Recieving;

    protected override void Awake()
    {

    }

    void Update()
    {
        if(StoredAmount == 0)
        {
            transform.LookAt(Shipping.PortPos.position);
            transform.position = Vector3.MoveTowards(transform.position, Shipping.PortPos.position, Speed * Time.deltaTime);
            if(transform.position == Shipping.PortPos.position)
            {
                Recieve(Shipping.Ship(ResourceToShip, Capacity));
            }
        }
        else
        {
            transform.LookAt(Recieving.PortPos.position);
            transform.position = Vector3.MoveTowards(transform.position, Recieving.PortPos.position, Speed * Time.deltaTime);
            if(transform.position == Recieving.PortPos.position)
            {
                Recieve(Recieving.Recieve(Ship(ResourceToShip, Capacity)));
            }
        }
    }
}
