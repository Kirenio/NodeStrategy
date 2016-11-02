using UnityEngine;
using System.Collections;

public class ExpandableMenu : MonoBehaviour {
    public Animator animator;
    public bool Closed = true;

    public void ChangeState()
    {
        if (Closed) animator.Play("BuildingMenuExpand");
        else animator.Play("BuildingMenuCollapse");
        Closed = !Closed;
    }
}