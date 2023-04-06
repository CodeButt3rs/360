using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GraphMenuButton : MonoBehaviour
{
    public GameObject GraphMenu;
    public GameObject Camera;
    private BasicRotation bR;
    private SceneGraph sG;
    private SceneManager sM;

    private void Start()
    {
        bR = Camera.GetComponent<BasicRotation>();
        sG = SceneGraph.Instance;
        sM = SceneManager.Instance; 
    }
    // Start is called before the first frame update
    public void OpenMenu()
    {
        GraphMenu.SetActive(!GraphMenu.activeSelf);
        sG.UpdateGraph(sM.Scenes);
        if (GraphMenu.activeSelf)
            DiasableBasicRotation();
        else
            EnableeBasicRotation();
    }
    public void DiasableBasicRotation()
        => bR.enabled = false;
    public void EnableeBasicRotation()
        => bR.enabled = true;
}
