using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypeChanged : MonoBehaviour
{
    TMP_Dropdown dropdown;
    public GameObject sceneCanvas;
    public GameObject informCanvas;

    public TMP_Dropdown scenesDropdown;

    [SerializeField]
    private Sprite infoTexture;
    [SerializeField]
    private Sprite transTexture;

    int previous = 0;
    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });
    }
    void DropdownValueChanged(TMP_Dropdown change)
    {
        Point point = SceneManager.Instance.CurrentPoint;
        //Debug.Log(gameObject.transform.parent.gameObject.name);
        //Debug.Log(change.value);
        if (change.value == 0 /*&& previous != 0*/)
        {
            informCanvas.SetActive(true);
            sceneCanvas.SetActive(false);
            SceneManager.Instance.CurrentScene.CastingTo(point.GameObject);
            point.GameObject.GetComponent<ScenesHandler>().SetInfoSprite();
        }
        if (change.value == 1 /*&& previous != 1*/)
        {
            informCanvas.SetActive(false);
            sceneCanvas.SetActive(true);
            SceneManager.Instance.CurrentScene.CastingTo(point.GameObject);

            TransitionPoint p = SceneManager.Instance.FindPoint(point.GameObject) as TransitionPoint;

            SceneManager.Instance.RefreshScenes();
            if (string.IsNullOrEmpty(scenesDropdown.itemText.text))
                p.TransitionScene = SceneManager.Instance.FindSceneByNameId(scenesDropdown.options[scenesDropdown.value].text);

            point.GameObject.GetComponent<ScenesHandler>().SetTransSprite();
        }
        previous = change.value;
    }
    public void SetListener() => dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });
}
