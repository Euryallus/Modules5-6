using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ExplodeOnImpact : MonoBehaviour
{
    private List<Material> addedBreakMaterials;
    private GameObject goDistortion;
    private Material distortionMat;
    private float explodeProgress;
    private bool exploding;

    // Update is called once per frame
    void Update()
    {
        if (exploding)
        {
            explodeProgress += Time.deltaTime * 7f;
            for (int i = 0; i < addedBreakMaterials.Count; i++)
            {
                addedBreakMaterials[i].SetFloat("_BreakProgress", explodeProgress);
                distortionMat.SetFloat("_FadeDistortion", explodeProgress);
            }

            if(explodeProgress >= 1f)
            {
                Destroy(gameObject);
                Destroy(goDistortion);
            }
        }
    }

    public void Explode()
    {
        if (!exploding)
        {
            explodeProgress = 0f;

            ReplaceMaterials();
            Instantiate(GameUtilities.instance.prefabBreakParticles, transform.position, Quaternion.identity);
            goDistortion = Instantiate(GameUtilities.instance.prefabDistortionSphere, transform.position, Quaternion.identity);
            distortionMat = goDistortion.GetComponent<MeshRenderer>().material;
            exploding = true;
        }
    }

    private void ReplaceMaterials()
    {
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
            Material newBreakMat = new Material(GameUtilities.instance.materialBreak);

            newBreakMat.SetColor("_Albedo", meshRenderer.material.GetColor("_BaseColor"));
            newBreakMat.SetFloat("_Metallic", meshRenderer.material.GetFloat("_Metallic"));
            newBreakMat.SetFloat("_Smoothness", meshRenderer.material.GetFloat("_Smoothness"));

            meshRenderer.material = newBreakMat;

            addedBreakMaterials.Add(newBreakMat);
        }
    }
}
