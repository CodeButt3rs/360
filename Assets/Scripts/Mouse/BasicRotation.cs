using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRotation : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(0f, 100f)]
    float rotationSpeed = 50.0f;
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 movement = new Vector3(
                Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime, 
                -1 * Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime,
                0
                );
            transform.eulerAngles += movement;
        }
    }
}
