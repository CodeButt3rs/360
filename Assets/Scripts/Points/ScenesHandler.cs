using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScenesHandler : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public SpriteRenderer spriteRenderer;
    public Sprite infoSprite;
    public Sprite transSprite;
    public Point point;
    private void Start()
    {
        point = SceneManager.Instance.FindPoint(gameObject);
    }
    public void ChangeScenes(List<Scene> scenes)
    {
        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
        foreach (var item in scenes)
            list.Add(new TMP_Dropdown.OptionData(item.Name + " #" + item.Id));
        dropdown.options = list;
    }
    public void SetInfoSprite() => spriteRenderer.sprite = infoSprite;
    public void SetTransSprite() => spriteRenderer.sprite = transSprite;
}
