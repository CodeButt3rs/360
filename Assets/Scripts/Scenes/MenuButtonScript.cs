using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
public class MenuButtonScript : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    GameObject sceneMenu;

    [SerializeField]
    GameObject sceneName;
    [SerializeField]
    GameObject sceneAuthor;
    [SerializeField]
    GameObject sceneImage;

    GameObject[] sceneMenuElements;

    public BasicRotation basicRotation;
    // Start is called before the first frame update
    private void Start()
    {
        sceneMenuElements = new GameObject[] { sceneName, sceneAuthor, sceneImage };
    }
    public void OpenMenu() {
        panel.SetActive(!panel.activeSelf);
        if (sceneMenu.activeSelf)
            SceneMenu();
        if(!panel.activeSelf)
            basicRotation.enabled = true;
    }
    public void SceneMenu()
    {
        sceneMenu.SetActive(!sceneMenu.activeSelf);
        foreach (var item in sceneMenuElements)
            item.GetComponent<TMP_InputField>().text = "";
    }
    public void CreateScene()
    {
        TMP_InputField name = sceneName.GetComponent<TMP_InputField>();
        TMP_InputField image = sceneImage.GetComponent<TMP_InputField>();
        TMP_InputField author = sceneAuthor.GetComponent<TMP_InputField>();
        Texture2D texture;
        if (string.IsNullOrEmpty(name.text) && name.text.Trim().Length == 0)
            return;
        if (string.IsNullOrEmpty(author.text) && author.text.Trim().Length == 0)
            return;
        if (string.IsNullOrEmpty(image.text) && image.text.Trim().Length == 0)
            return;
        WWW www = new WWW(image.text);
        Debug.Log(image.text);
        while (!www.isDone)
            continue;
        texture = www.texture;
        SceneManager.Instance.CreateScene(name.text, Random.Range(0, 1000).ToString(), texture);
        SceneMenu();
    }
    public void DeletePointButton() =>
        SceneManager.Instance.DeletePoint(SceneManager.Instance.CurrentPoint.GameObject);
}
