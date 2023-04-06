using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTest : MonoBehaviour
{
    public HideAndShow current;
    public HideAndShow previous;
    public GameObject prefabPoint;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SceneManager.Instance.CreatePoint(transform);

        }
    }
}
