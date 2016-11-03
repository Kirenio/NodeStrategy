using UnityEngine;
using UnityEngine.UI;

public class SelectionMenu : MonoBehaviour {
    public Controls controls;

    [Header("Selection menu")]
    public Text MenuHeader;
    public Toggle ShippingButton;
    public Text ShippingButtonText;
    public Toggle RecievingButton;
    public Text RecievingButtonText;
    public Dropdown ResourceButton;
    public Text ResourceButtonText;
    public GameObject ResourceChangeIndicator;
    public Toggle PauseButton;
    public Image ReturnCargoButton;
    int TargetToChange;
    CargoCart cart;
    Building building;

    [Header("Unit menu")]
    public GameObject InventoryMenu;
    public Text InventoryHeader;
    public Slider InventoryBar;
    public Text InventoryText;
    public Text Stats;

    void Awake () {
        controls.CargoCartSelected += fillMenu;
        controls.BuildingSelected += fillMenu;
        controls.Unselected += hideMenu;

        ResourceChangeIndicator.SetActive(false);
        ResourceButton.ClearOptions();
        foreach (Resource res in Resource.GetValues(typeof(Resource)))
        {
            ResourceButton.options.Add(new Dropdown.OptionData(res.ToString()));
        }
        hideMenu();
	}
	
    void fillMenu(CargoCart value)
    {
        hideMenu(); // Making sure building will not override inventory if cargo cart was selected without unselecting building first and vise versa
                    // (!) Need a better solution for the whole unite menu section

        // Filling the Selection menu
        cart = value;
        controls.ShiftPressed += FastTargeting;

        gameObject.SetActive(true);
        MenuHeader.text = cart.name;
        ShippingButtonText.text = cart.ShippingName;
        RecievingButtonText.text = cart.RecievingName;
        PauseButton.isOn = cart.Paused;
        cart.PauseEbabled += updateConditionalButtons;
        ReturnCargoButton.enabled = cart.ReturningCargo;

        ResourceChangeIndicator.SetActive(cart.ResourceChangePending);
        if (cart.ResourceChangePending)
        {
            ResourceButtonText.text = cart.ResourceToShipNew.ToString();
            cart.ResourceToShipChanged += ResourceToShipChanged;
        }
        else ResourceButtonText.text = cart.ResourceToShip.ToString();

        // Filling the Unit menu
        InventoryMenu.SetActive(true);
        InventoryHeader.text = cart.name + " inventory";
        updateInventoryCart();
        cart.InventoryChanged += updateInventoryCart;
    }

    void fillMenu(Building value)
    {
        hideMenu();

        building = value;

        // Filling the Unit menu
        InventoryMenu.SetActive(true);
        InventoryHeader.text = building.name + " inventory";
        updateInventoryBuilding();
        building.InventoryChanged += updateInventoryBuilding;
    }

    void updateInventoryCart()
    {
        InventoryBar.maxValue = cart.Capacity;
        InventoryBar.value = cart.StoredAmount;
        InventoryText.text = cart.GetContentString();
        Stats.text = cart.GetStatsString();
    }

    void updateInventoryBuilding()
    {
        InventoryBar.maxValue = building.Capacity;
        InventoryBar.value = building.StoredAmount;
        InventoryText.text = building.GetContentString();
        Stats.text = building.GetStatsString();
    }

    void updateConditionalButtons()
    {
        PauseButton.isOn = cart.Paused;
        ReturnCargoButton.enabled = cart.ReturningCargo;
    }

    void hideMenu()
    {
        gameObject.SetActive(false);
        InventoryMenu.SetActive(false);
        if (cart != null)
        {
            cart.InventoryChanged -= updateInventoryCart;
            cart.ResourceToShipChanged -= ResourceToShipChanged;
            controls.ShiftPressed -= FastTargeting;
            cart.PauseEbabled -= updateConditionalButtons;
        }
        if (building != null) building.InventoryChanged -= updateInventoryBuilding;
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

    public void OrderReturnCargo()
    {
        Debug.Log("Ordering return of the cargo!");
        cart.ReturningCargo = !cart.ReturningCargo;
        ReturnCargoButton.enabled = true;
    }

    public void PauseCart()
    {
        cart.Paused = PauseButton.isOn;
    }

    public void ChangeResourceToShip()
    {
        Debug.Log("Asking CargoCart to queue a new resource to ship!");
        cart.QueueResourceChange((Resource)ResourceButton.value);
        ResourceChangeIndicator.SetActive(true);
        cart.ResourceToShipChanged += ResourceToShipChanged;
    }

    void ResourceToShipChanged()
    {
        ResourceChangeIndicator.SetActive(false);
        cart.ResourceToShipChanged -= ResourceToShipChanged;
    }

    public void ChangeTarget(int value)
    {
        controls.CargoCartSelected -= fillMenu; // Disabling the normal selection behaviour
        controls.BuildingSelected -= fillMenu;

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

        controls.CargoCartSelected += fillMenu;
        controls.BuildingSelected += fillMenu;
    }

    void SetTargetFast(Building target)
    {
        switch (TargetToChange)
        {
            case 0:
                cart.Shipping = target;
                ShippingButtonText.text = cart.Shipping.name;
                ShippingButton.isOn = false;
                RecievingButton.isOn = true; // Setting up for the next change
                TargetToChange = 1;
                break;
            case 1:
                cart.Recieving = target;
                RecievingButtonText.text = cart.Recieving.name;
                RecievingButton.isOn = false;
                FastTargetingStop();
                break;
        }
    }

    void FastTargeting()
    {
        Debug.Log("Fast targeting is on");
        controls.ShiftPressed -= FastTargeting;
        controls.ShiftReleased += FastTargetingStop; // Subscribing in case player releases the shift button

        controls.CargoCartSelected -= fillMenu; // Disabling the normal selection behaviour
        controls.BuildingSelected -= fillMenu;
        controls.Unselected -= hideMenu;
        ShippingButton.isOn = true; // Highlighting the button to show what will be changed

        TargetToChange = 0; // Setting the default target to change
        controls.BuildingSelected += SetTargetFast;
        controls.InTargetSelectionMode = true;
    }

    void FastTargetingStop()
    {
        Debug.Log("Fast targeting is off");
        controls.ShiftReleased -= FastTargetingStop;
        controls.ShiftPressed += FastTargeting;

        controls.BuildingSelected -= SetTargetFast;
        controls.InTargetSelectionMode = false;

        ShippingButton.isOn = false;
        RecievingButton.isOn = false;

        controls.CargoCartSelected += fillMenu; // Finaly return the normal selection behaviour
        controls.BuildingSelected += fillMenu;
        controls.Unselected += hideMenu;
    }
}
