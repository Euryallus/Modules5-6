using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float moveSensitivity;
    public Transform transformPlayer;

    private float cameraXRotation;  //For looking up/down
    private float playerYRotation;  //For looking left/right

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseXAxis = Input.GetAxis("Mouse X") * moveSensitivity * Time.deltaTime;
        float mouseYAxis = Input.GetAxis("Mouse Y") * moveSensitivity * Time.deltaTime;

        cameraXRotation -= mouseYAxis;
        cameraXRotation = Mathf.Clamp(cameraXRotation, -90f, 90f);

        playerYRotation -= mouseXAxis;

        //Rotate camera
        transform.localRotation = Quaternion.Euler(cameraXRotation, 0f, 0f);

        //Rotate the player body
        transformPlayer.localRotation = Quaternion.Euler(0f, -playerYRotation, 0f);
    }
}
