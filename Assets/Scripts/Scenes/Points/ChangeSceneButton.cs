using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeSceneButton : MonoBehaviour
{
    public TMP_InputField name;
    public TMP_InputField author;
    
    public void OpenSceneChangeMenu()
    {
        name.text = SceneManager.Instance.CurrentScene.Name;
        author.text = SceneManager.Instance.CurrentScene.Author;
    }
    public void OpenSceneChangeMenu(string _name, string _author)
    {
        name.text = _name;
        author.text = _author;
    }
    public void SaveSceneChanges() =>
        SceneManager.Instance.ChangeSceneData(name.text, author.text);
    public void SaveSceneChanges(Scene s) =>
        SceneManager.Instance.ChangeSceneData(s, name.text, author.text);
    public void DeleteScene()
    {
        if (SceneManager.Instance.Scenes.Count != 1)
            SceneManager.Instance.DeleteScene();
    }
    
}
