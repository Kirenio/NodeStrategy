using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    Building Selection;
    public Text UnitInfo;
    public SelectionPathTracking PathTracker;
    
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (Selection != null) Selection.InventoryChanged -= UpdateToolTip;
                if (hit.transform.tag == "Moveable")
                {
                    PathTracker.SetCartToTrack(hit.transform.GetComponent<CargoCart>());
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
                    if (Selection != null) Selection.InventoryChanged -= UpdateToolTip;
                    Selection = null;
                    UnitInfo.transform.parent.gameObject.SetActive(false);
                    PathTracker.gameObject.SetActive(false);
                }
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Building target = hit.transform.gameObject.GetComponent<Building>();
                if (target != null) target.LogContent();
            }
        }
    }

    void UpdateToolTip()
    {
        UnitInfo.text = string.Format("{0}\n{1}/{2} HP\n{3}/{4} Space", Selection.name, (int)Selection.CurrentHealth, Selection.MaxHealth, (int)Selection.StoredAmount, Selection.Capacity);
    }
}