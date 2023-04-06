using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ContentEvents : MonoBehaviour
{
    public RectTransform contentRectTransform;
    public float scrollSpeed = 3.0f;
    private void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        contentRectTransform = GetComponent<RectTransform>();
    }
}
