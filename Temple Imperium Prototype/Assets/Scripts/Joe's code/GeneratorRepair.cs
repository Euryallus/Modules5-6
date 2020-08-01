using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

//------------------------------------------------------\\
//  Manages the game mechanic of repairing the generator\\
//  by clicking on it with collected GeneratorPieces    \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//      and modified/optimised for prototype phase      \\
//------------------------------------------------------\\

public class GeneratorRepair : MonoBehaviour
{
    [Header("Add pieces in order of repair:")]

    //Set in inspector:
    [SerializeField]
    [Tooltip("The pieces that the player must collect to repair the generator")]
    private GeneratorPiece[] piecesForRepair;
    [SerializeField]
    [Tooltip("Whether the player has to collect pieces in the order they are listed")]
    private bool mustCollectInOrder;
    [SerializeField]
    private GameObject goPiecesUIPanel;         //The UI panel that contains all collected pieces
    [SerializeField]
    private GameObject goRepairProgressUIPanel; //The UI panel showing how close the generator is to being repaired
    [SerializeField]
    private GameObject prefabUIPiecePreview;    //The prefab UI panel used to show each collected piece
    [SerializeField]
    private string repairSound;                 //The sound to be played when repairing the generator

    private Queue<GeneratorPiece> collectedPieceQueue = new Queue<GeneratorPiece>();    //Collected generator pieces, using queue so items
                                                                                        //can be removed in the order they were added
    private bool generatorRepaired = false; //Whether the generator is currently repaired
    private int currentCollectionProgress;  //The number of pieces that have been collected
    private int currentRepairProgress;      //How many pieces have been added to the generator
    private GameObject goPlayer;            //The player whose position determines if they are close enough to the generator to repair it
    private bool mouseOverGenerator;        //Whether the player is mousing over the generator
    private bool canClickGenerator;         //Whether the player is in a valid position where the generator can be clicked
    private Slider repairProgressSlider;    //SLider showing how close the generator is to being repaired
    private bool endlessMode;

    public bool GetGeneratorRepaired()
    {
        //In endless mode, the generator does not need to be repaired. As such, GetGeneratorRepaired
        //   always returns true so the player is not incorrectly penalised
        if (endlessMode)
        {
            return true;
        }
        else { return generatorRepaired; }
    }

    void Start()
    {
        //Find the player and UI slider in the current scene
        goPlayer = GameObject.FindGameObjectWithTag("Player");
        repairProgressSlider = goRepairProgressUIPanel.transform.Find("Slider").GetComponent<Slider>();

        //Hide the collected pieces UI since none are collected yet
        goPiecesUIPanel.transform.parent.gameObject.SetActive(false);

        //Check if playing in endless mode
        endlessMode = (PlayerPrefs.GetInt("EndlessMode", 0) == 1);

        if (endlessMode)
        {
            //Remove all generator pieces if in endless mode to avoid
            //  confusion since the player does not need to collet them
            for (int i = 0; i < piecesForRepair.Length; i++)
            {
                Destroy( piecesForRepair[i].gameObject );
            }
        }
        else
        {
            //Set repairIndex for each piece to determine their collection order
            for (int i = 0; i < piecesForRepair.Length; i++)
            {
                piecesForRepair[i].repairIndex = i;
            }
        }
    }

    public bool TryCollectPiece(GeneratorPiece piece, string collectionSoundName)
    {
        if (endlessMode)
            return false;

        //All pieces to be picked up should be added to this script
        if(!piecesForRepair.Contains(piece))
        {
            Debug.LogWarning("Generator piece not added to GeneratorRepair script: " + piece.GetPieceName());
            return false;
        }

        //Check if this piece can be collected
        //  If pieces must be collected in order and the piece's index does not match the repair index, it cannot be collected
        bool canCollect;
        if(mustCollectInOrder)
        {
            if(piece == piecesForRepair[currentCollectionProgress])
            {
                canCollect =  true;
            }
            else
            {
                canCollect =  false;
            }
        }
        else
        {
            canCollect = true;
        }

        if (canCollect)
        {
            //The piece can be collected - add it to the collected piece queue...
            collectedPieceQueue.Enqueue(piece);

            //  ...and the collection UI
            goPiecesUIPanel.transform.parent.gameObject.SetActive(true);
            GameObject goPiecePreview = Instantiate(prefabUIPiecePreview, goPiecesUIPanel.transform);
            goPiecePreview.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = piece.GetPieceName();
            piece.goUIPreview = goPiecePreview;

            //Increase collection progress unless all pieces were collected
            if (currentCollectionProgress < (piecesForRepair.Length - 1))
            {
                currentCollectionProgress++;
            }
            else
            {
                Debug.Log("All generator pieces collected");
            }

            //Play the collection sound for audio feedback
            AudioManager.instance.PlaySoundEffect2D(collectionSoundName);
        }

        return canCollect;
    }

    void Update()
    {
        //Check if the player if in a position where they can click the generator
        bool prevCanClickGenerator = canClickGenerator;
        canClickGenerator = (mouseOverGenerator && Vector3.Distance(goPlayer.transform.position, gameObject.transform.position) < 5f);

        //If canClickGenerator changed since the last frame, update whether the player is showing
        //  an empty hand or weapon. This stops the player being able to shoot the generator when repairing
        if (canClickGenerator != prevCanClickGenerator)
        {
            goPlayer.GetComponent<WeaponHolder>().SetEmptyHand(canClickGenerator);
        }

        //If the generator can be clicked, show repair progress UI. Otherwise hide it
        if (canClickGenerator)
        {
            goRepairProgressUIPanel.SetActive(true);
            repairProgressSlider.value = (float)currentRepairProgress / piecesForRepair.Length;
        }
        else
        {
            goRepairProgressUIPanel.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (canClickGenerator)
        {
            //Clicking the generator while holding at least 1 piece
            if(collectedPieceQueue.Count > 0)
            {
                //Remove the piece from UI and the collection queue since it has been used
                GeneratorPiece pieceToAdd = collectedPieceQueue.Dequeue();
                Destroy(pieceToAdd.goUIPreview);

                //Increase the generator's repair progress
                currentRepairProgress++;

                //Hide collection UI if all pieces were used
                if(collectedPieceQueue.Count == 0)
                {
                    goPiecesUIPanel.transform.parent.gameObject.SetActive(false);
                }
                AudioManager.instance.PlaySoundEffect2D(repairSound);
            }
            if(currentRepairProgress == piecesForRepair.Length)
            {
                //If all pieces were added to the generator, mark it as repaired
                Debug.Log("Generator repaired");
                generatorRepaired = true;
            }
        }
    }

    private void OnMouseEnter()
    {
        if(!endlessMode)
        {
            mouseOverGenerator = true;
        }
    }
    private void OnMouseExit()
    {
        mouseOverGenerator = false;
    }
}
