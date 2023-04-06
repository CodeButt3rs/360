using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
public class SceneUIScript : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _name;
    [SerializeField]
    private TMP_Text _id;
    [SerializeField]
    private Image _panel;
    [SerializeField]
    private Image _transPoint;
    [SerializeField]
    private TMP_Text _startPos;

    [SerializeField]
    private ChangeSceneButton CSB;
    public void InitializeUIScene(string name, string id)
    {
        Scene s = SceneManager.Instance.FindSceneById(id);
        _name.text = name;
        _id.text = id;
        
        _startPos.text = "" + s.StartPitch.ToString().Substring(0, s.StartPitch.ToString().Length > 1 ? 5 : 1) + "\n" + s.StartYaw.ToString().Substring(0, s.StartYaw.ToString().Length > 1 ? 5 : 1) + "";
    }
    public void CurrentScene() =>
        _panel.color = new Color(0, 0, 0, 0.5f);
    public void SetTransition() =>
        _transPoint.color = Color.white;
    public void ChangeSceneClick(BaseEventData d)
    {
        SceneManager.Instance.InitializeScene(SceneManager.Instance.FindSceneById(_id.text));
    }
}
