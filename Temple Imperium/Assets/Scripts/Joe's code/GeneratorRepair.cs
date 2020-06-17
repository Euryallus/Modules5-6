using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorRepair : MonoBehaviour
{
    [Header("Add pieces in order of repair:")]

    [SerializeField]
    [Tooltip("The pieces that the player must collect to repair the generator")]
    private GeneratorPiece[] piecesForRepair;

    [SerializeField]
    [Tooltip("Whether the player has to collect pieces in the order they are listed")]
    private bool mustCollectInOrder;

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
        bool canCollect;
        if(mustCollectInOrder)
        {
            if(piece == piecesForRepair[currentRepairProgress])
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
            if(currentRepairProgress < (piecesForRepair.Length - 1))
            {
                currentRepairProgress++;
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
        }
    }

    private void OnMouseDown()
    {
        if (canClickGenerator)
        {
            Debug.Log("REPAIR");
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
