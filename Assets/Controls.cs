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
                if (hit.transform.tag == "Moveable")
                {
                    PathTracker.SetCartToTrack(hit.transform.GetComponent<CargoCart>());
                }

                Selection = hit.transform.GetComponent<Building>();
                if (Selection != null) UnitInfo.text = string.Format("{0}\n{1}/{2} HP\n{3}/{4} Space",Selection.name,Selection.CurrentHealth,Selection.MaxHealth,Selection.StoredAmount,Selection.Capacity);
                else
                {
                    Selection = null;
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
}