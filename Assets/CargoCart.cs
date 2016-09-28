using UnityEngine;
using System.Collections;

public class CargoCart : Building {
    public float Speed;

    public Building Shipping;
    public Building Recieving;

    void Update()
    {
        if(Stored.Amount < Capacity)
        {
            transform.LookAt(Shipping.PortPos.position);
            transform.position = Vector3.MoveTowards(transform.position, Shipping.PortPos.position, Speed * Time.deltaTime);
            if(transform.position == Shipping.PortPos.position)
            {
                Stored = Shipping.Ship(Capacity);
            }
        }
        else
        {
            transform.LookAt(Recieving.PortPos.position);
            transform.position = Vector3.MoveTowards(transform.position, Recieving.PortPos.position, Speed * Time.deltaTime);
            if(transform.position == Recieving.PortPos.position)
            {
                Stored = Recieving.Recieve(Stored);
            }
        }
    }
}
