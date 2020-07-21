using UnityEngine;

//------------------------------------------------------\\
//  Basic player script used for testing purposes       \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

[RequireComponent(typeof(WeaponHolder))]
public class Player : MonoBehaviour
{
    //Set in inspector:
    public CharacterController characterController; //Used for character movement
    public float moveSpeed;                         //How quickly the player moves

    void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        //Move the player based on player input
        float axisHorizontal = Input.GetAxis("Horizontal");
        float axisVertical = Input.GetAxis("Vertical");
        Vector3 moveVector = transform.right * axisHorizontal + transform.forward * axisVertical;
        //Using Time.deltaTime so player speed is framerate independent
        characterController.Move(moveVector * moveSpeed * Time.deltaTime);
    }
}
