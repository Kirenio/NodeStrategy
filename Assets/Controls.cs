using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Building target = (Building)hit.transform.gameObject.GetComponent("Building");
                if (target != null) target.LogContent();
            }
        }
    }
}