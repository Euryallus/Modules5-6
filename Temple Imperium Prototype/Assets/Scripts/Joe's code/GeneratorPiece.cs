using UnityEngine;

//------------------------------------------------------\\
//  A piece that can be collected by the player and     \\
//  used to repair the generator                        \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class GeneratorPiece : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private string pieceName;   //The name of this piece to display in the UI
    [SerializeField]
    private string pickupSound; //Name of the sound to be played when this piece is picked up

    public int repairIndex { get; set; }        //Used to check if this piece should be collected before/after others
    public GameObject goUIPreview { get; set; } //UI to show that this piece was collected

    public string GetPieceName() { return pieceName; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Check the GeneratorManager is placed in the scene
            GameObject goGeneratorManager = GameObject.FindGameObjectWithTag("GeneratorManager");
            if(goGeneratorManager == null)
            {
                Debug.LogError("GeneratorManager prefab has not been placed in scene");
                return;
            }

            //Check the GeneratorRepair script exists
            GeneratorRepair generatorRepair = goGeneratorManager.GetComponent<GeneratorRepair>();
            if(generatorRepair == null)
            {
                Debug.LogError("GeneratorRepair script not found on GeneratorManager");
                return;
            }

            //If this is a valid piece to collect, collect it and destroy the GameObject
            //  so it can't be collected multiple times
            if (generatorRepair.TryCollectPiece(this, pickupSound))
            {
                Debug.Log("Collected GeneratorPiece: " + pieceName);
                Destroy(gameObject);
            }
            else
            {
                //This piece was collected out of order
                Debug.Log("Can't collect GeneratorPiece: " + pieceName + " - collected out of order");
            }
        }
    }
}
