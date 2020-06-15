using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{

    float yVelocity;
    float mouseX;
    float mouseY;

    float rotateY = 0;

    float inputX;
    float inputY;
    Vector3 moveTo;

    float defaultSpeed;

    bool onLadder = false;

    [Header("Basic control options")]
        [SerializeField]
        float playerJump = 1f;
        [SerializeField]
        float mouseSensitivity = 500f;
        [SerializeField]
        float playerSpeed = 10f;
        [SerializeField]
        float playerMass = 1;
        [SerializeField]
        float sprintMag = 1.75f;
        [SerializeField]
        float crouchSpeedReduction = 2;

    private bool isCrouching = false;
    private float gravConst = 9.81f;
    private bool hasJumped = false;
    GameObject playerCamera;
    CharacterController controller;
    bool canClimb = false;

    private GameObject noteMenu;
    void Start()
    {
        playerCamera = gameObject.transform.GetChild(0).gameObject;
        defaultSpeed = playerSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        controller = gameObject.GetComponent<CharacterController>();

        noteMenu = GameObject.FindGameObjectWithTag("noteDisplayManager");
    }

    // Update is called once per frame
    void Update()
    {
        moveCamera();
        movePlayer();
    }

    void moveCamera()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotateY -= mouseY;
        rotateY = Mathf.Clamp(rotateY, -75f, 75f);

        playerCamera.transform.localRotation = Quaternion.Euler(rotateY, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void movePlayer()
    {

        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        if (controller.isGrounded)
        {
            hasJumped = false;
            yVelocity = -gravConst * Time.deltaTime;
        }
        else
        {
            if(hasJumped == false && onLadder == false)
            {
                hasJumped = true;
            }
            yVelocity -= gravConst * Time.deltaTime * playerMass;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(hasJumped == false)
            {
                yVelocity = playerJump;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playerSpeed *= sprintMag;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            playerSpeed /= sprintMag ;
        }

        if (onLadder != true)
        {
            moveTo = transform.right * inputX + transform.forward * inputY;
            moveTo.y = yVelocity;
        }
        else
        {
            moveTo = transform.up * inputY ;
        }

        controller.Move(moveTo * playerSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.C))
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

        if(canClimb && Input.GetKeyDown(KeyCode.E))
        {
            onLadder = !onLadder;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            noteMenu.GetComponent<CanvasGroup>().alpha = 1;
        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            noteMenu.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    public void crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
        playerSpeed /= crouchSpeedReduction;
    }

    public void standUp()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
        playerSpeed = defaultSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collided = other.transform.gameObject;

        if (collided.CompareTag("Ladder"))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject collided = other.transform.gameObject;

        if (collided.CompareTag("Ladder"))
        {
            canClimb = false;
            onLadder = false;
        }
    }
}
