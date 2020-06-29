using System.Collections.Generic;
using UnityEngine;

//------------------------------------------------------\\
//  Add this to a GameObject to allow it to be          \\
//  destroyed by the player with a weapon               \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ExplodeOnImpact : MonoBehaviour
{
    private List<Material> addedBreakMaterials; //Materials that will be applied when the object is hit
    private GameObject goDistortion;            //GameObject with distortion effect shader
    private Material distortionMat;             //Material used for distortion effect
    private float explodeProgress;              //Progress for the explosion effect (0 = normal, 1 = fully exploded)
    private bool exploding;                     //Whether the object is currently exploding

    // Update is called once per frame
    void Update()
    {
        if (exploding)
        {
            //If exploding, increase the explode progress that determines
            //  how broken the object will look
            explodeProgress += Time.deltaTime * 7f;
            for (int i = 0; i < addedBreakMaterials.Count; i++)
            {
                //Update material shader parameters with the explosion progress
                addedBreakMaterials[i].SetFloat("_BreakProgress", explodeProgress);
                distortionMat.SetFloat("_FadeDistortion", explodeProgress);
            }

            if(explodeProgress >= 1f)
            {
                //If done exploding, destroy this object and the distortion effect
                Destroy(gameObject);
                Destroy(goDistortion);
            }
        }
    }

    public void Explode()
    {
        if (!exploding)
        {
            //Reset explodeProgress so the object starts by looking unbroken
            explodeProgress = 0f;

            //Replace standard materials with break materials so the effect can be applied
            ReplaceMaterials();
            //Spawn a particle effect to enhance the break effect
            Instantiate(GameUtilities.instance.prefabBreakParticles, transform.position, Quaternion.identity);
            //Spawn a GameObject with a distortion shader to enhance the break effect
            goDistortion = Instantiate(GameUtilities.instance.prefabDistortionSphere, transform.position, Quaternion.identity);
            distortionMat = goDistortion.GetComponent<MeshRenderer>().material;
            //Set exploding to true so the break effect can start
            exploding = true;
        }
    }

    private void ReplaceMaterials()
    {
        //Replaces the materials of the gameObject and its children
        //  so the break effect can be applied
        addedBreakMaterials = new List<Material>();
        TryReplaceMaterial(gameObject);
        foreach (Transform child in transform)
        {
            TryReplaceMaterial(child.gameObject);
        }
    }

    private void TryReplaceMaterial(GameObject gameObj)
    {
        MeshRenderer meshRenderer = gameObj.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            //Create a new break material to apply to the current GameObject
            Material newBreakMat = new Material(GameUtilities.instance.materialBreak);
            //Set paramaters based on the object's current material so it looks
            //  visually similar when the effect starts
            newBreakMat.SetColor("_Albedo", meshRenderer.material.GetColor("_BaseColor"));
            newBreakMat.SetFloat("_Metallic", meshRenderer.material.GetFloat("_Metallic"));
            newBreakMat.SetFloat("_Smoothness", meshRenderer.material.GetFloat("_Smoothness"));
            //Apply the break material
            meshRenderer.material = newBreakMat;
            addedBreakMaterials.Add(newBreakMat);
        }
    }
}
