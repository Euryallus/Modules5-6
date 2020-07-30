using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// ## HUGO BAILEY
// ## Written: Prototype phase
// ## Purpose: Control active Star Stone and manage charge & rotations (improvement of generatorStates)
//
public class starStoneManager : MonoBehaviour
{
    [Header("UI textures")]
    [SerializeField]
    [Tooltip("IN ORDER: BLUE, PINK, ORANGE, PURPLE, DEFAULT")]

    //Index of item in list all depends on Enum value (e.g. Blue has index 0, so blue texture must be at enemyMaterials[0])
    private List<Texture> UIMaterials = new List<Texture>();
    [SerializeField]
    private List<Image> UIElements = new List<Image>();
    [SerializeField]
    private List<GameObject> UIHighlights = new List<GameObject>();
    [SerializeField]
    private List<Material> enemyMaterials = new List<Material>();
    private List<float> activeCharge = new List<float> {0, 0, 0, 0};
    private Texture currentTexture;

    [SerializeField]
    private CanvasGroup starStoneHUD;

    [Header("Star stone charge details")]
    [SerializeField]
    private float maxCharge = 20;

    private void Start()
    {
        //sets charge of all star stones to max charge (stored as maxCharge)
        for (int i = 0; i < activeCharge.Count; i++)
        {
            activeCharge[i] = maxCharge;
        }
    }

    public enum starStones //possible star stone states
    {
        Blue,
        Pink,
        Orange,
        Purple,
        None
    }

    protected starStones activeStone = starStones.None; //initialised to None

    public starStones returnActive() //returns active star stone (used in prototype weapon script and enemy damage)
    {
        return activeStone;
    }

    public void activateStone(starStones stone) //sets active star stone to stone passed in parameters (used in pickUpControlScript)
    {
        if(activeStone == starStones.None)
        {
            starStoneHUD.alpha = 1; //disables HUD if no stone is selected (default)
        }

        //sets active stone to stone passed
        activeStone = stone;
        //sets texture to be altered to the active stone's colour
        currentTexture = UIMaterials[(int)activeStone];
        // e.g. if Pink is active, display only the pink material

    }

    public void FixedUpdate()
    {
        for (int i = 0; i < activeCharge.Count; i++) //cycles all star stone charges 
        {
            if(i == (int)activeStone) //if the currently inspected stone is the one that's active
            {
                if (activeCharge[(int)activeStone] > 0)
                {
                    activeCharge[(int)activeStone] -= Time.deltaTime; //reduce it's charge if it's not below or = to 0
                }
                else
                {
                    int nextStone = (int)activeStone + 1; //if charge on the active stone is 0, cycle to the next one in the list
                    if(nextStone == 4) //circular list implementation
                    {
                        nextStone = 0;
                    }

                    activeStone = (starStones)nextStone;
                }

                UIHighlights[i].SetActive(true); //highlight the UI element that corrosponds to the active stone
            }
            else
            {
                UIHighlights[i].SetActive(false); //don't highlight the UI element for a stone that's disabled
                if (activeCharge[i] < maxCharge)
                {
                    activeCharge[i] += Time.deltaTime; //increase the charge of the unused stone (to max of maxCharge)
                }
            }

            //alter alpha of stone's UI element to match current charge
            UIHighlights[i].GetComponent<Image>().color = new Color(UIHighlights[i].GetComponent<Image>().color.r, UIHighlights[i].GetComponent<Image>().color.g, UIHighlights[i].GetComponent<Image>().color.b, activeCharge[i] / maxCharge);

            //
            UIElements[i].color = new Color(UIElements[i].color.r, UIElements[i].color.g, UIElements[i].color.b, activeCharge[i] / maxCharge); 
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //creates list of all alive enemies

        if(enemies.Length > 0 && GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().returnState() == playStateControl.waveState.waveActive)
        {
            // if the game is running & enemies are alive, alter their material to match which stone is active
            for (int j = 0; j < enemies.Length; j++)
            {
                if(enemies[j].GetComponent<Enemy>().hasHurt == false)
                {
                    enemies[j].GetComponent<MeshRenderer>().material = enemyMaterials[(int)activeStone];
                }
                
            }
        }
        
    }
}
