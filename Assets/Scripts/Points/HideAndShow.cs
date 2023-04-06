using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class HideAndShow : MonoBehaviour
{
    public GameObject mainCanvas;
    public Point point;
    private void OnMouseUpAsButton()
    {
        SceneManager.Instance.ChangePointInfo(transform.root.gameObject);
    }
    private void OnMouseDrag()
    {
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            SceneManager.Instance.FindPoint(transform.root.gameObject).MovePoint();
    }
}
