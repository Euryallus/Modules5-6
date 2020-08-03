using UnityEngine;

//------------------------------------------------------\\
//  Forces sprites to always face towards the camera    \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class SpriteFaceCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //Force sprite to rotate towards the camera
        transform.LookAt(Camera.main.transform, Vector3.up);
    }
}
