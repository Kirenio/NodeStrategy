using UnityEngine;
using System.Collections;

public class SelectionPathTracking : MonoBehaviour
{
    public Controls controls;

    CargoCart SelectedObject;
    LineRenderer LRPointer;
    
    void Awake()
    {
        LRPointer = gameObject.GetComponent<LineRenderer>();
        gameObject.SetActive(false);
        controls.CargoCartSelected += SetCartToTrack;
        controls.Unselected += StopTracking;
    }

	void Update () {
        LRPointer.SetPosition(1, SelectedObject.transform.position);
    }

    void SetCartToTrack(CargoCart cart)
    {
        SelectedObject = cart;
        LRPointer.SetPosition(0, SelectedObject.Shipping.transform.position);
        LRPointer.SetPosition(2, SelectedObject.Recieving.transform.position);
        gameObject.SetActive(true);
    }

    void StopTracking()
    {
        gameObject.SetActive(false);
    }
}
