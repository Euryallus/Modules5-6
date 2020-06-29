using UnityEngine;

//------------------------------------------------------\\
//  Used for doors that can be locked/unlocked.         \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class Door : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private bool locked;                //Whether the door is currently locked (i.e. blocking the player)
    [SerializeField]
    private Animator animator;          //The animator used for visually opening/closing the door
    [SerializeField]
    private BoxCollider boxCollider;    //The collider used for blocking the player when the door is locked

    public void SetLocked(bool locked)
    {
        //Lock or unlock the door, e.g. can be used for unlocking a door after a certain wave
        this.locked = locked;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //If the player enters the door's trigger radius, either open the door
            //  or show a 'door locked' popup if the door is currently locked
            if (!locked)
            {
                //Disable the collider so the player can move through
                boxCollider.enabled = false;
                //Visually open the door
                animator.SetBool("Open", true);
            }
            else
            {
                UIManager.instance.ShowDoorLockedPopup(boxCollider.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Ensure the door is closed and the locked popup is not visible
        //  when the player leaves the door's trigger radius
        if (other.CompareTag("Player"))
        {
            boxCollider.enabled = true;
            animator.SetBool("Open", false);

            UIManager.instance.HideDoorLockedPopup();
        }
    }
}
