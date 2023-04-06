using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStartPosButton : MonoBehaviour
{
    public void SetStartPosButton_Click()
    {
        SceneManager.Instance.CurrentScene.SetStartPos();
        SceneManager.Instance.ScenesList();
    }
    
}
