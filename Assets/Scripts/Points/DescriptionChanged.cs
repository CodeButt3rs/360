using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DescriptionChanged : MonoBehaviour
{
    void Start() =>
        GetComponent<TMP_InputField>().onEndEdit.AddListener(DescriptionHandler);
    
    void DescriptionHandler(string text)
    {
        InfoPoint point = SceneManager.Instance.CurrentPoint as InfoPoint;
        point.ChangeDescription(text);
    }
}
