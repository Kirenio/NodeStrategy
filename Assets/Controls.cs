using UnityEngine;
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
    CargoCart selectionCart;
    public string BuildingToBuild { get;  set; }
    public bool InTargetSelectionMode = false;
    [Header("Prefabs of game buildings")]
    public ConstructionSite ConstructionSitePrefab;
    public Extractor ExtractorPrefab;
    public Silo SiloPrefab;
    public Manufactory ManufactoryPrefab;
    public Warehouse WarehousePrefab;
    public Extractor BlackSandExtractorPreafab;

    bool movingCamera = false;

    public event ControlsEvenHandler Unselected;
    public event ControlsEvenHandler ShiftPressed;
    public event ControlsEvenHandler ShiftReleased;
    public event ControlsEvenHandlerBuilding BuildingCreated;
    public event ControlsEvenHandlerBuilding BuildingSelected;
    public event ControlsEvenHandlerCargoCart CargoCartSelected;
    
    void Update()
    {
        // TODO: Rewrite this to be uniform
        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Moveable")
                {
                    selectionCart = hit.transform.GetComponent<CargoCart>();
                    if (CargoCartSelected != null) CargoCartSelected(selectionCart);
                    return;
                }

                Selection = hit.transform.GetComponent<Building>();
                if (Selection != null)
                {
                    if(BuildingSelected != null) BuildingSelected(Selection);
                }
                else
                {
                    Selection = null;
                    if (Unselected != null) Unselected();
                }
            }
        }

        // Debug feature
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

        // TODO: Come up with terrain/resource structure and rewrite this into MouseButton 0
        if (Input.GetMouseButtonUp(1)) 
        {
            if (EventSystem.current.IsPointerOverGameObject() || movingCamera)
            {
                movingCamera = false;
                return;
            }

            if (BuildingToBuild != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Debug.Log("Hit tag: "+ hit.transform.tag);
                    if(hit.transform.tag == "Ground")
                    {
                        Debug.Log("Placing: " + BuildingToBuild);
                        ConstructionSite newBuilding = ((ConstructionSite)Instantiate(ConstructionSitePrefab, hit.point, Quaternion.identity)).GetComponent<ConstructionSite>();
                        newBuilding.SetOrder(this);
                        if (BuildingCreated != null) BuildingCreated(newBuilding);
                    }
                }
            }
        }

        // Camera pan
        if (Input.GetMouseButton(1))
        {
            Vector3 change = new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * mouseSensitivity;
            if (change != Vector3.zero)
            {
                movingCamera = true;
                CameraAnchor.position += change;
            }
        }
        if (Input.GetKey(KeyCode.W)) CameraAnchor.position += Vector3.forward * keyboardScrollSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) CameraAnchor.position += -Vector3.forward * keyboardScrollSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) CameraAnchor.position += -Vector3.right * keyboardScrollSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) CameraAnchor.position += Vector3.right * keyboardScrollSpeed * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (ShiftPressed != null)
            {
                Debug.Log("Shift pressed!");
                ShiftPressed();
            }
        }
        
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (ShiftReleased != null)
            {
                Debug.Log("Shift released!");
                ShiftReleased();
            }
        }

        // Camera zoom
        if (!EventSystem.current.IsPointerOverGameObject()) Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollMultiplier,1,20);
    }

    // Teleport camera to specified position
    // TODO: make an optional slow transition between current position and target
    public void TransferCamera(Vector3 value)
    {
        CameraAnchor.position = new Vector3(value.x, 0, value.z - 45);
    }
}