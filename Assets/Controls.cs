using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void ControlsEvenHandler();
public delegate void ControlsEvenHandlerBuilding(Building sender);
public delegate void ControlsEvenHandlerCargoCart(CargoCart sender);

public class Controls : MonoBehaviour
{
    public Transform CameraAnchor;
    public int scrollMultiplier;
    public float mouseSensitivity = 0.3f;
    public float keyboardScrollSpeed = 2f;
    Building Selection;
    public Text UnitInfo;
    public SelectionPathTracking PathTracker;
    public GameObject BuildingToBuild { get;  set;  }

    public event ControlsEvenHandler Unselected;
    public event ControlsEvenHandlerBuilding BuildingSelected;
    public event ControlsEvenHandlerCargoCart CargoCartSelected;
    
    void Update()
    {

        if (Input.GetKey(KeyCode.W)) CameraAnchor.position += Vector3.forward * keyboardScrollSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) CameraAnchor.position += -Vector3.forward * keyboardScrollSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) CameraAnchor.position += -Vector3.right * keyboardScrollSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) CameraAnchor.position += Vector3.right * keyboardScrollSpeed * Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            CameraAnchor.position += new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * mouseSensitivity;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (Selection != null) Selection.InventoryChanged -= UpdateToolTip;
                if (hit.transform.tag == "Moveable")
                {
                    CargoCart selectedCart = hit.transform.GetComponent<CargoCart>();
                    PathTracker.SetCartToTrack(selectedCart);
                    if (CargoCartSelected != null) CargoCartSelected(selectedCart);
                }

                Selection = hit.transform.GetComponent<Building>();
                if (Selection != null)
                {
                    UnitInfo.transform.parent.gameObject.SetActive(true);
                    Selection.InventoryChanged += UpdateToolTip;
                    UpdateToolTip();
                }
                else
                {
                    if (Selection != null)
                    {
                        Selection.InventoryChanged -= UpdateToolTip;
                    }
                    Selection = null;
                    if (Unselected != null) Unselected();
                    UnitInfo.transform.parent.gameObject.SetActive(false);
                    PathTracker.gameObject.SetActive(false);
                }
            }
        }

        if (Input.GetMouseButtonUp(2))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Building target = hit.transform.gameObject.GetComponent<Building>();
                if (target != null) target.LogContent();
            }
        }

        if(Input.GetMouseButtonUp(1))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (BuildingToBuild != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Instantiate(BuildingToBuild, hit.point, Quaternion.identity);
                }
            }
        }
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollMultiplier,1,20);
    }

    void UpdateToolTip()
    {
        UnitInfo.text = string.Format("{0}\n{1}/{2} HP\n{3}/{4} Space", Selection.name, (int)Selection.CurrentHealth, Selection.MaxHealth, (int)Selection.StoredAmount, Selection.Capacity);
    }

    public void TransferCamera(Vector3 value)
    {
        CameraAnchor.position = new Vector3(value.x, 0, value.z - 45);
    }
}