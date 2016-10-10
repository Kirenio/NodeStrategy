using UnityEngine;
using System.Collections;

public class ExpandableMenu : MonoBehaviour {
    public Animator animator;
    bool state = true;

    public void ChangeState()
    {
        if (state) animator.Play("BuildingMenuExpand");
        else animator.Play("BuildingMenuCollapse");
        state = !state;
    }
}