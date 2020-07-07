using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbArea : MonoBehaviour
{
    [SerializeField]
    private ReverbAreas reverbAreaContainer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            reverbAreaContainer.TriggerReverb(gameObject.name);
        }
    }
}
