using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseOffScript : MonoBehaviour
{
    Image img;
    private GameObject cameraMovement;
    private void TurnOffMovement(BaseEventData d)
    {
        cameraMovement.GetComponent<BasicRotation>().enabled = true;
    }
    private void TurnOnMovement(BaseEventData d)
    {
        cameraMovement.GetComponent<BasicRotation>().enabled = true;
    }
}
