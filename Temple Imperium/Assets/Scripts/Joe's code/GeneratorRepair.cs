using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class GeneratorRepair : MonoBehaviour
{
    [Header("Add pieces in order of repair:")]

    [SerializeField]
    [Tooltip("The pieces that the player must collect to repair the generator")]
    private GeneratorPiece[] piecesForRepair;

    [SerializeField]
    [Tooltip("Whether the player has to collect pieces in the order they are listed")]
    private bool mustCollectInOrder;

    [SerializeField]
    private GameObject goPiecesUIPanel;

    [SerializeField]
    private GameObject goRepairProgressUIPanel;

    [SerializeField]
    private GameObject prefabUIPiecePreview;

    [SerializeField]
    private string pieceCollectSound;
    [SerializeField]
    private string repairSound;

    private Queue<GeneratorPiece> collectedPieceQueue = new Queue<GeneratorPiece>();
    private bool generatorReapired;
    private int currentCollectionProgress;
    private int currentRepairProgress;
    private GameObject goPlayer;
    private bool mouseOverGenerator;
    private bool canClickGenerator;
    private Slider repairProgressSlider;

    // Start is called before the first frame update
    void Start()
    {
        goPlayer = GameObject.FindGameObjectWithTag("Player");
        repairProgressSlider = goRepairProgressUIPanel.transform.Find("Slider").GetComponent<Slider>();

        goPiecesUIPanel.transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < piecesForRepair.Length; i++)
        {
            piecesForRepair[i].repairIndex = i;
        }
    }

    public bool GetGeneratorRepaired()
    {
        return generatorReapired;
    }

    public bool TryCollectPiece(GeneratorPiece piece)
    {
        if(!piecesForRepair.Contains(piece))
        {
            Debug.LogWarning("Generator piece not added to GeneratorRepair script: " + piece.GetPieceName());
            return false;
        }

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
            SoundEffectPlayer.instance.PlaySoundEffect2D(pieceCollectSound);

            goPiecesUIPanel.transform.parent.gameObject.SetActive(true);

            GameObject goPiecePreview = Instantiate(prefabUIPiecePreview, goPiecesUIPanel.transform);
            goPiecePreview.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = piece.GetPieceName();
            piece.goUIPreview = goPiecePreview;

            collectedPieceQueue.Enqueue(piece);

            if (currentCollectionProgress < (piecesForRepair.Length - 1))
            {
                currentCollectionProgress++;
            }
            else
            {
                generatorReapired = true;
                Debug.Log("ALL GENERATOR PIECES COLLECTED!");
            }
        }

        return canCollect;
    }

    // Update is called once per frame
    void Update()
    {
        bool prevCanClickGenerator = canClickGenerator;
        canClickGenerator = (mouseOverGenerator && Vector3.Distance(goPlayer.transform.position, gameObject.transform.position) < 5f);
        
        if(canClickGenerator != prevCanClickGenerator)
        {
            goPlayer.GetComponent<WeaponHolder>().SetEmptyHand(canClickGenerator);
        }

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
            if(collectedPieceQueue.Count > 0)
            {
                SoundEffectPlayer.instance.PlaySoundEffect2D(repairSound);

                GeneratorPiece pieceToAdd = collectedPieceQueue.Dequeue();
                Destroy(pieceToAdd.goUIPreview);
                currentRepairProgress++;

                if(collectedPieceQueue.Count == 0)
                {
                    goPiecesUIPanel.transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        mouseOverGenerator = true;
    }

    private void OnMouseExit()
    {
        mouseOverGenerator = false;
    }
}
