using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase & edited prototype phase
// ## Purpose: Manages player's health & associated effects
//

public class playerMovement : MonoBehaviour
{
    [Header("Basic control options")]
        [SerializeField]
        private float playerJump = 1f;
        [SerializeField]
        private float mouseSensitivity = 500f;
        [SerializeField]
        private float playerSpeed = 10f;
        [SerializeField]
        private float playerMass = 1;
        [SerializeField]
        private float sprintMag = 1.75f;
        [SerializeField]
        private float crouchSpeedReduction = 2;

    private GameObject noteMenu;
    private GameObject head;
    private GameObject playerCamera;
    private CharacterController controller;

    private Vector3 moveTo;

    private float yVelocity;
    private float mouseX;
    private float mouseY;
    private float rotateY = 0;
    private float inputX;
    private float inputY;
    private float defaultSpeed;
    private float gravConst = 9.81f;

    private bool canClimb = false;
    private bool onLadder = false;
    private bool isCrouching = false;
    private bool hasJumped = false;
    private bool isSlowed = false;
    private bool playingFootstepSound = false;
    private float footstepTimer = 0f;

    void Start()
    {
        playerCamera = gameObject.transform.GetChild(0).gameObject;
        defaultSpeed = playerSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        controller = gameObject.GetComponent<CharacterController>();

        Time.timeScale = 1;

        noteMenu = GameObject.FindGameObjectWithTag("noteDisplayManager");
        head = GameObject.FindGameObjectWithTag("head");

        GameObject.FindGameObjectWithTag("navMesh").GetComponent<NavMeshSurface>().BuildNavMesh();
    }


    void Update()
    {
        moveCamera();
        movePlayer();

        UpdateFootstepSounds();
    }

    void moveCamera()
    {
        //
        // ## CONTROLS CAMERA ROTATION
        // ## Takes mouse X and Y input and translates into rotation (Y clamped between -75 and 75)
        // ## Camera rotation affected by Y input, player rotation affected by X input
        // 
        // ## VIDEO LOOSLY FOLLOWED: https://youtu.be/_QajrabyTJc 2019 - Brackeys "FIRST PERSON MOVEMENT in Unity - FPS Controller"
        //

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotateY -= mouseY;
        rotateY = Mathf.Clamp(rotateY, -75f, 75f);

        playerCamera.transform.localRotation = Quaternion.Euler(rotateY, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void movePlayer()
    {
        //
        // ## PLAYER MOVEMENT CONTROLS
        // ## Mouse WASD input used to determine characterController movement
        // 
        // ## VIDEO LOOSLY FOLLOWED: https://youtu.be/_QajrabyTJc 2019 - Brackeys "FIRST PERSON MOVEMENT in Unity - FPS Controller"
        //

        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        if (controller.isGrounded) //once player hits the ground, resets jump condition and sets gravity to -9.81
        {
            hasJumped = false;
            yVelocity = -gravConst * Time.deltaTime;
        }
        else
        {
            if(hasJumped == false && onLadder == false)  
            {
                hasJumped = true; //sets jumped to true if player has just left the ground, sets Y position to itself - scaled gravity
                // Character controller has no programmed gravity, hence implementing it manually
            }
            yVelocity -= gravConst * Time.deltaTime * playerMass;
        }

        if (Input.GetKeyDown(KeyCode.Space)) //when space is pressed, y velocity = upward velocity defined in inspector
        {
            if(hasJumped == false)
            {
                yVelocity = playerJump;
            }
        }

        //Handles sprint conditions - alters depending on whether or not the player is affected by slow conditions or not
        if (Input.GetKeyDown(KeyCode.LeftShift))  //begin sprint
        {
            if(isSlowed == false)
            {
                playerSpeed = defaultSpeed * sprintMag;
            }
            else
            {
                playerSpeed = playerSpeed * sprintMag;
            }
        }

        if(Input.GetKeyUp(KeyCode.LeftShift)) //end sprint
        {
            if(isSlowed == false)
            {
                playerSpeed = defaultSpeed;
            }
            else
            {
                playerSpeed = playerSpeed / sprintMag;

            }
            
        }

        if (onLadder != true) //alters how Y input is handled based on whether or not player is on ladder:
        {                     // if on ladder, Y controls "forward" velocity, else it controls "up" velocity
            moveTo = transform.right * inputX + transform.forward * inputY;
            moveTo.y = yVelocity;
        }
        else
        {
            moveTo = transform.up * inputY ;
        }

        controller.Move(moveTo * playerSpeed * Time.deltaTime); //applies movement to player

        if (Input.GetKeyDown(KeyCode.C)) //toggles crouching (reduces verticle scale of player by 1/2)
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                crouch();
            }
            else
            {
                standUp();
            }
        }

        if(canClimb && Input.GetKeyDown(KeyCode.E)) //if player is within ladder trigger volume & presses E, either enter or exit the ladder
        {
            onLadder = !onLadder;
        }

        if (Input.GetKeyDown(KeyCode.I)) //while I is held, display the note menu
        {
            noteMenu.GetComponent<CanvasGroup>().alpha = 1;
        }
        if (Input.GetKeyUp(KeyCode.I)) //when I is released, hide display menu
        {
            noteMenu.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    //Added by Joe, handles footstep sounds based on player speed:
    private void UpdateFootstepSounds()
    {
        //Allow footstep sounds to play if the player is moving in any direction, using a threshold
        //  of 0.3 to prevent sounds playing for tiny movements
        if (Mathf.Abs(inputX) > 0.3f || Mathf.Abs(inputY) > 0.3f && !canClimb && !hasJumped)
        {
            if (!playingFootstepSound)
            {
                playingFootstepSound = true;
            }
        }
        //If the player is not walking, or they are climbing/jumping, do not allow footstep sounds to play
        else
        {
            playingFootstepSound = false;
            footstepTimer = 0f;
        }

        //Play a random footstep sound every time the footstep timer reaches 0
        if (playingFootstepSound)
        {
            footstepTimer -= Time.deltaTime;
            if(footstepTimer <= 0f)
            {
                AudioManager.instance.PlaySoundEffect2D("Footstep" + Random.Range(1, 6), 0.8f, 0.5f, 1.05f);
                //After playing a sound, reset  the timer based on player speed so the next sound
                //  is triggered after a set time based on how quickly the player is moving
                footstepTimer = 2f / playerSpeed;
            }
        }
    }

    public void crouch() //crouching recudes speed by predetermined %
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
        playerSpeed /= crouchSpeedReduction;
        head.transform.localScale = new Vector3(head.transform.localScale.x, head.transform.localScale.y * 2, head.transform.localScale.z);
    }

    public void standUp() //returns scale & speed to previous speed
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
        playerSpeed = defaultSpeed;
        head.transform.localScale = new Vector3(head.transform.localScale.x, head.transform.localScale.y / 2, head.transform.localScale.z);
    }

    private void OnTriggerEnter(Collider other) //detects when wnters ladder trigger volume
    {
        GameObject collided = other.transform.gameObject;

        if (collided.CompareTag("Ladder"))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit(Collider other) //detects when exits ladder trigger volume
    {
        GameObject collided = other.transform.gameObject;

        if (collided.CompareTag("Ladder"))
        {
            canClimb = false;
            onLadder = false;
        }
    }

    public void slowEffect(float percentageSlow, float time) //initiates speed reduction
    {
        if(isSlowed == false) 
        {
            gameObject.GetComponent<playerHealth>().stateDisplay.text = "slowed!"; //updates UI
            isSlowed = true;
            playerSpeed = playerSpeed *  (1 - percentageSlow); // reduces speed by predetermined %

            StartCoroutine(returnToNormalSpeed(time)); //begins countdown to return to normal speed based on parameter passed
        }
        
    }

    private IEnumerator returnToNormalSpeed(float time)
    {
        yield return new WaitForSeconds(time); 
        
        isSlowed = false; //resets speed reduction condition
        gameObject.GetComponent<playerHealth>().stateDisplay.text = ""; //alters UI
        playerSpeed = defaultSpeed; //resets speed

    }
}
