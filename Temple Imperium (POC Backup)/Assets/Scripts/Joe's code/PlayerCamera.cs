using UnityEngine;

//------------------------------------------------------\\
//  Basic player first person camera used for testing   \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class PlayerCamera : MonoBehaviour
{
    //Set in inspector:
    public float moveSensitivity;       //How quickly the camera moves based on mouse movement
    public Transform transformPlayer;   //Player transform for tracking the player's position

    private float cameraXRotation;  //Rotation used for looking up/down
    private float playerYRotation;  //Rotation used for looking left/right

    void Update()
    {
        //Get mouse x/y input, using Time.deltaTime to make camera movement framerate independent
        float mouseXAxis = Input.GetAxis("Mouse X") * moveSensitivity * Time.deltaTime;
        float mouseYAxis = Input.GetAxis("Mouse Y") * moveSensitivity * Time.deltaTime;

        //Set camera and player rotation, clamping to ensure the player cannot look behind themselves
        cameraXRotation -= mouseYAxis;
        cameraXRotation = Mathf.Clamp(cameraXRotation, -90f, 90f);
        playerYRotation -= mouseXAxis;

        //Rotate the camera
        transform.localRotation = Quaternion.Euler(cameraXRotation, 0f, 0f);
        //Rotate the player body
        transformPlayer.localRotation = Quaternion.Euler(0f, -playerYRotation, 0f);
    }
}
