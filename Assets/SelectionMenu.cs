using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectionMenu : MonoBehaviour {
    public Controls controls;

    public Text MenuHeader;
    public Text ShippingButtonText;
    public Text RecievingButtonText;
    public Text ResourceButtonText;
    CargoCart cart;

    void Awake () {
        controls.CargoCartSelected += fillMenu;
        controls.Unselected += hideMenu;
	}
	
    void fillMenu(CargoCart value)
    {
        cart = value;
        gameObject.SetActive(true);
        MenuHeader.text = cart.name;
        ShippingButtonText.text = cart.Shipping.name;
        RecievingButtonText.text = cart.Recieving.name;
        ResourceButtonText.text = cart.ResourceToShip.ToString();
    }

    void hideMenu()
    {
        gameObject.SetActive(false);
    }

    public void Locate(int value)
    {
        switch(value)
        {
            case 0:
                controls.TransferCamera(cart.Shipping.transform.position);
                break;
            case 1:
                controls.TransferCamera(cart.Recieving.transform.position);
                break;
        }
    }

    public void OrderDumpCargo()
    {
        cart.DumpCargo();
    }
}
