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
    private List<Texture> UIMaterials = new List<Texture>();
    [SerializeField]
    private List<Image> UIElements = new List<Image>();
    private List<float> activeCharge = new List<float> {0, 0, 0, 0};
    private Texture currentTexture;

    [SerializeField]
    private CanvasGroup starStoneHUD;

    [Header("Star stone charge details")]
    [SerializeField]
    private float maxCharge = 20;

    private void Start()
    {
        for (int i = 0; i < activeCharge.Count; i++)
        {
            activeCharge[i] = maxCharge;
        }
    }

    public enum starStones
    {
        Blue,
        Pink,
        Orange,
        Purple,
        None
    }

    protected starStones activeStone = starStones.None;

    public starStones returnActive()
    {
        return activeStone;
    }

    public void activateStone(starStones stone)
    {
        if(activeStone == starStones.None)
        {
            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().startGame();
            starStoneHUD.alpha = 1;
        }

        activeStone = stone;
        currentTexture = UIMaterials[(int)activeStone];

    }

    public void FixedUpdate()
    {
        for (int i = 0; i < activeCharge.Count; i++)
        {
            if(i == (int)activeStone)
            {
                if (activeCharge[(int)activeStone] > 0)
                {
                    activeCharge[(int)activeStone] -= Time.deltaTime;
                }
                else
                {
                    int nextStone = (int)activeStone + 1;
                    if(nextStone == 4)
                    {
                        nextStone = 0;
                    }

                    activeStone = (starStones)nextStone;
                }
            }
            else
            {
                if (activeCharge[i] < maxCharge)
                {
                    activeCharge[i] += Time.deltaTime;
                }
            }

            UIElements[i].color = new Color(UIElements[i].color.r, UIElements[i].color.g, UIElements[i].color.b, activeCharge[i] / maxCharge);
        } 
    }
}
