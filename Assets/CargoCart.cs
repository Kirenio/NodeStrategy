using UnityEngine;
using System.Collections;

public class CargoCart : Building {
    public float Speed;

    public Resource ResourceToShip;

    public Building Shipping;
    public Building Recieving;

    void Update()
    {
        if(Stored.Amount == 0)
        {
            transform.LookAt(Shipping.PortPos.position);
            transform.position = Vector3.MoveTowards(transform.position, Shipping.PortPos.position, Speed * Time.deltaTime);
            if(transform.position == Shipping.PortPos.position)
            {
                Stored = Shipping.Ship(ResourceToShip, Capacity);
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
