using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float time = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, time);
    }
}
