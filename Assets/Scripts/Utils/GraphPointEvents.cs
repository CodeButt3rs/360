using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class GraphPointEvents : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public SceneGraph sG;
    public TMP_Text sceneName;
    public GraphPoint gP;
    private BasicRotation bR;
    RectTransform rT;
    void Start()
    {
        rT = GetComponent<RectTransform>();
        sG = SceneGraph.Instance;
        bR = SceneManager.Instance.UserCamera.GetComponent<BasicRotation>();
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        gP = sG.FindPoint(gameObject);
        bR.enabled = false;
        rT.anchoredPosition += eventData.delta;
        gP.RenderLines();
    }
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        sG.FindPoint(gameObject).HighlightConnected();
    }
    void IEndDragHandler.OnEndDrag(PointerEventData eventData) =>
        sG.FindPoint(gameObject).UnhighlightConnected();
    public void SetSceneName(string name) => sceneName.text = name;
}
