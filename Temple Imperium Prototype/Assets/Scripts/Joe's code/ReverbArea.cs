using UnityEngine;

//------------------------------------------------------\\
//  Triggers reverb when in a certain area of the map   \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class ReverbArea : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private ReverbAreas reverbAreaContainer;

    private void OnTriggerEnter(Collider other)
    {
        //When the player enters this ReverbArea, trigger reverb
        //  passing the attatched GameObject's name to specify reverb type
        if (other.CompareTag("Player"))
        {
            reverbAreaContainer.TriggerReverb(gameObject.name);
        }
    }
}
