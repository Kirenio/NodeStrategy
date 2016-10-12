using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectionMenu : MonoBehaviour {
    public Controls controls;

    public Text MenuHeader;
    public Toggle ShippingButton;
    public Text ShippingButtonText;
    public Toggle RecievingButton;
    public Text RecievingButtonText;
    public Dropdown ResourceButton;
    public Toggle PauseButton;
    int TargetToChange;
    CargoCart cart;

    void Awake () {
        controls.CargoCartSelected += fillMenu;
        controls.Unselected += hideMenu;

        ResourceButton.ClearOptions();
        foreach (Resource res in Resource.GetValues(typeof(Resource)))
        {
            ResourceButton.options.Add(new Dropdown.OptionData(res.ToString()));
        }
	}
	
    void fillMenu(CargoCart value)
    {
        cart = value;
        gameObject.SetActive(true);
        MenuHeader.text = cart.name;
        ShippingButtonText.text = cart.Shipping.name;
        RecievingButtonText.text = cart.Recieving.name;
        ResourceButton.value = (int)cart.ResourceToShip;
        PauseButton.isOn = cart.Paused;
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

    public void PauseCart()
    {
        cart.Paused = PauseButton.isOn;
    }

    public void ChangeResourceToShip()
    {
        cart.ResourceToShip = (Resource)ResourceButton.value;
    }

    public void ChangeTarget(int value)
    {
        if (ShippingButton.isOn || RecievingButton.isOn)
        {
            Debug.Log("Setting new target");
            controls.BuildingSelected += SetTarget;
            controls.InTargetSelectionMode = true;
            TargetToChange = value;
        }
        else
        {
            Debug.Log("Canceling new target");
            controls.BuildingSelected -= SetTarget;
            controls.InTargetSelectionMode = false;
        }
    }

    void SetTarget(Building target)
    {
        switch(TargetToChange)
        {
            case 0:
                cart.Shipping = target;
                ShippingButtonText.text = cart.Shipping.name;
                ShippingButton.isOn = false;
                break;
            case 1:
                cart.Recieving = target;
                RecievingButtonText.text = cart.Recieving.name;
                RecievingButton.isOn = false;
                break;
        }
    }
}
