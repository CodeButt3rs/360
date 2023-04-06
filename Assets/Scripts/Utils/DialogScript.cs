using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AnotherFileBrowser.Windows;
using TMPro;

public class DialogScript : MonoBehaviour
{
    [SerializeField]
    GameObject pano;
    // Start is called before the first frame update
    public void button1_Click()
    {
        var bp = new BrowserProperties();
        bp.filter = "Равнопромежуточная проекция (*.jpg, *.png) | *.jpg; *.png";
        new FileBrowser().OpenFileBrowser(bp, path => { GetComponent<TMP_InputField>().text = path; });
    }
    IEnumerator ChangeTexture(string image)
    {
        WWW www = new WWW(image);
        while (!www.isDone)
            yield return null;
        pano.GetComponent<Renderer>().material.mainTexture = www.texture;
    }
}
