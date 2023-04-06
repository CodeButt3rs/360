using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TransitionSceneChanged : MonoBehaviour
{
    private void Start()
    {
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(delegate { ValueHandler(dropdown); });
        SceneManager.Instance.ScenesList();
    }
    void ValueHandler(TMP_Dropdown change)
    {
        TransitionPoint point = SceneManager.Instance.FindPoint(SceneManager.Instance.CurrentPoint.GameObject) as TransitionPoint;
        Debug.Log(point.Name + " " + point.TransitionSceneId + " " + SceneManager.Instance.CurrentPoint.Name + " " + change.options[change.value].text);
        point.SetTransitionScene(change.options[change.value].text);
        SceneManager.Instance.ScenesList();
    }
}
