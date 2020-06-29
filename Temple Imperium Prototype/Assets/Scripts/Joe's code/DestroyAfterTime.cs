using UnityEngine;

//------------------------------------------------------\\
//  Add this to a GameObject to allow it to be          \\
//  automatically destroyed after a set amount of time. \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class DestroyAfterTime : MonoBehaviour
{
    public float time = 1f;     //Amount of time before the object will be destroyed

    void Start()
    {
        //Destroy the object after it has been in existence for (time) seconds
        Destroy(gameObject, time);
    }
}
