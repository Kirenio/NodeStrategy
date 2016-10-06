using UnityEngine;
using System.Collections;

public class SelectionPathTracking : MonoBehaviour {
    CargoCart SelectedObject;
    LineRenderer LRPointer;

    public void SetCartToTrack(CargoCart cart)
    {
        SelectedObject = cart;
        LRPointer.SetPosition(0, SelectedObject.Shipping.transform.position);
        LRPointer.SetPosition(2, SelectedObject.Recieving.transform.position);
        gameObject.SetActive(true);
    }

    void Awake()
    {
        LRPointer = gameObject.GetComponent<LineRenderer>();
        gameObject.SetActive(false);
    }

	void Update () {
        LRPointer.SetPosition(1, SelectedObject.transform.position);
    }
}
