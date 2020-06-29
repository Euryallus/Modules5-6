using UnityEngine;

public class Door : MonoBehaviour
{
    //Set in inspector:

    [SerializeField]
    private bool locked;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private BoxCollider boxCollider;

    public void SetLocked(bool locked)
    {
        this.locked = locked;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!locked)
            {
                boxCollider.enabled = false;
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
        if (other.CompareTag("Player"))
        {
            boxCollider.enabled = true;
            animator.SetBool("Open", false);

            UIManager.instance.HideDoorLockedPopup();
        }
    }
}
