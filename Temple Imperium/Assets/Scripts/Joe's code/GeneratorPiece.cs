using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorPiece : MonoBehaviour
{
    [SerializeField]
    private string pieceName;

    public int repairIndex { get; set; }

    public string GetPieceName()
    {
        return pieceName;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject goGeneratorManager = GameObject.FindGameObjectWithTag("GeneratorManager");

            if(goGeneratorManager == null)
            {
                Debug.LogError("GeneratorManager prefab has not been placed in scene");
                return;
            }

            GeneratorRepair generatorRepair = goGeneratorManager.GetComponent<GeneratorRepair>();

            if(generatorRepair == null)
            {
                Debug.LogError("GeneratorRepair script not found on GeneratorManager");
                return;
            }

            if (generatorRepair.TryCollectPiece(this))
            {
                Debug.Log("Collected GeneratorPiece: " + pieceName);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Can't collect GeneratorPiece: " + pieceName + " - collected out of order");
            }
        }
    }
}
