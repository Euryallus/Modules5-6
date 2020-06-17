using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

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

    private Queue<GeneratorPiece> collectedPieceQueue = new Queue<GeneratorPiece>();
    private int currentCollectionProgress;
    private int currentRepairProgress;
    private GameObject goPlayer;
    private bool mouseOverGenerator;
    private bool canClickGenerator;

    // Start is called before the first frame update
    void Start()
    {
        goPlayer = GameObject.FindGameObjectWithTag("Player");

        for (int i = 0; i < piecesForRepair.Length; i++)
        {
            piecesForRepair[i].repairIndex = i;
        }
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
                Debug.Log("ALL GENERATOR PIECES COLLECTED!");
            }
        }

        return canCollect;
    }

    // Update is called once per frame
    void Update()
    {
        canClickGenerator = (mouseOverGenerator && Vector3.Distance(goPlayer.transform.position, gameObject.transform.position) < 5f);
        
        if (canClickGenerator)
        {
            goRepairProgressUIPanel.SetActive(true);
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
            GeneratorPiece pieceToAdd = collectedPieceQueue.Dequeue();
            Destroy( pieceToAdd.goUIPreview );
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
