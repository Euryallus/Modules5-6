using UnityEngine;

//------------------------------------------------------\\
//  Used for doors that can be locked/unlocked based    \\
//  on the current wave                                 \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//      and modified/optimised for prototype phase      \\
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
    [SerializeField]
    private bool openForEnemies = true; //If true, door opening can be triggered by enemies as well as the player

    private int entityInDoorCount;      //The number of entities within the door's trigger radius, used to ensure
                                        //  that doors only close when there are no entities still using them

    public void SetLocked(bool locked)
    {
        //Lock or unlock the door, e.g. can be used for unlocking a door after a certain wave
        this.locked = locked;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || (openForEnemies && other.CompareTag("Enemy")))
        {
            //If a player (or enemy if the door is also open for enemies) enters the trigger radius,
            //  increase the counter of entities using the door
            entityInDoorCount++;

            //If the player enters the door's trigger radius, either open the door
            //  or show a 'door locked' popup if the door is currently locked
            if (!locked)
            {
                OpenDoor();
            }

            //If the door is locked and the player tries to use it
            //  display a UI popup notifying them of this
            if(locked && other.CompareTag("Player"))
            {
                UIManager.instance.ShowDoorLockedPopup(boxCollider.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Ensure the door is closed and the locked popup is not visible
        //  when the player leaves the door's trigger radius
        if (other.CompareTag("Player") || (openForEnemies && other.CompareTag("Enemy")))
        {
            //Decrease the counter of entities using the door
            entityInDoorCount--;

            //Close the door if no entities are now using it
            if(entityInDoorCount <= 0)
            {
                CloseDoor();
            }

            //Ensure the door locked UI popup is hidden once
            //  the player moves away from the door
            if (other.CompareTag("Player"))
            {
                UIManager.instance.HideDoorLockedPopup();
            }
        }
    }

    private void OpenDoor()
    {
        //Disable the collider entities can move through
        boxCollider.enabled = false;
        //Visually open the door
        animator.SetBool("Open", true);
    }

    private void CloseDoor()
    {
        //There should always be 0 entities using a door when it closes,
        //  if this is somehow not the case, the counter is reset to prevent
        //  further problems/unintended behaviour
        entityInDoorCount = 0;

        //Enable the collider entities cannot move through
        boxCollider.enabled = true;
        //Visually close the door
        animator.SetBool("Open", false);
    }
}
