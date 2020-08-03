using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Control active Star Stone and manage charge & rotations
//

public class generatorStates : MonoBehaviour
{
    private GameObject[] enemies;

    public enum starStoneActive
    {
        None,
        Purple,
        Orange,
        Blue,
        Pink
    }

    private starStoneActive activeStone = starStoneActive.None;

    [Header("Enemy materials")]
        [SerializeField]
        Material blue;
        [SerializeField]
        Material orange;
        [SerializeField]
        Material pink;
        [SerializeField]
        Material purple;
        [SerializeField]
        Material normal;

    float BlueActive = 0;
    float OrangeActive = 0;
    float PinkActive = 0;
    float PurpleActive = 0;

    [Header("UI Elements")]
        [SerializeField]
        CanvasGroup icons;
        [SerializeField]
        Text bluedisplay;
        [SerializeField]
        Text orangedisplay;
        [SerializeField]
        Text pinkdisplay;
        [SerializeField]
        Text purpledisplay;

    [Header("Gameplay elements")]
        [SerializeField]
        float rechargeDelay = 2;
        [SerializeField]
        float stoneActiveTime = 20f;

    public starStoneActive returnState()
    {
        // returns active star stone on call
        return activeStone;
    }

    public void FixedUpdate()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        switch (activeStone)
        {
            case starStoneActive.Blue:
                //
                // ## BLUE STAR STONE
                // Decreases Blue's charge, on 0 being reached cycles to Orange
                // Sets enemies colour to Blue material
                //

                BlueActive += Time.deltaTime;

                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i].GetComponent<MeshRenderer>().material = blue;
                }

                if (BlueActive > stoneActiveTime)
                {
                    activateOrange();
                }

                break;

            case starStoneActive.Orange:
                //
                // ## ORANGE STAR STONE
                // Decreases Blue's charge, on 0 being reached cycles to Pink
                // Sets enemies colour to Orange material
                //

                OrangeActive += Time.deltaTime;

                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i].GetComponent<MeshRenderer>().material = orange;
                }

                if (OrangeActive > stoneActiveTime)
                {
                    activatePink();
                }

                break;

            case starStoneActive.Pink:
                //
                // ## PINK STAR STONE
                // Decreases Blue's charge, on 0 being reached cycles to Purple
                // Sets enemies colour to Pink material
                //

                PinkActive += Time.deltaTime;

                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i].GetComponent<MeshRenderer>().material = pink;
                }

                if (PinkActive > stoneActiveTime)
                {
                    activatePurple();

                }

                break;

            case starStoneActive.Purple:
                //
                // ## PURPLE STAR STONE
                // Decreases Blue's charge, on 0 being reached cycles to Blue
                // Sets enemies colour to Purple material
                //

                PurpleActive += Time.deltaTime;

                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i].GetComponent<MeshRenderer>().material = purple;
                }

                if (PurpleActive > stoneActiveTime)
                {
                    activateBlue();
                }

                break;

            case starStoneActive.None:
                //
                // ## DEFAULT STATE
                //

                icons.alpha = 0;
                break;
        }

        //
        // RECHARGE
        // Each Star Stone that isn't currently active will regain charge (recharge rate is dampened by 'rechargeDelay' %
        // Updated charge is displayed on UI
        //

        if (activeStone != starStoneActive.Purple)
        {
            if(PurpleActive > 0)
            {
                PurpleActive -= Time.deltaTime / rechargeDelay;
                purpledisplay.text = (stoneActiveTime - PurpleActive).ToString("F1");
            }
        }

        if (activeStone != starStoneActive.Pink)
        {
            if(PinkActive > 0)
            {
                PinkActive -= Time.deltaTime / rechargeDelay;
                pinkdisplay.text = (stoneActiveTime - PinkActive).ToString("F1");
            }
        }

        if (activeStone != starStoneActive.Blue)
        {
            if (BlueActive > 0)
            {
                BlueActive -= Time.deltaTime / rechargeDelay;
                bluedisplay.text = (stoneActiveTime - BlueActive).ToString("F1");
            }
        }

        if (activeStone != starStoneActive.Orange)
        {
            if (OrangeActive > 0)
            {
                OrangeActive -= Time.deltaTime / rechargeDelay;
                orangedisplay.text = (stoneActiveTime - OrangeActive).ToString("F1");

            }
        }
    }

    public void activatePurple()
    {
        //
        // Changes active state to Purple
        // Updates UI, if game isn't currently started it begins on stone selection
        //

        icons.alpha = 1;
        if(activeStone == starStoneActive.None)
        {
            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().startGame();
        }

        activeStone = starStoneActive.Purple;
        purpledisplay.text = "Active";
        
    }

    public void activateOrange()
    {
        //
        // Changes active state to Orange
        // Updates UI, if game isn't currently started it begins on stone selection
        //

        icons.alpha = 1;

        if (activeStone == starStoneActive.None)
        {
            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().startGame();
        }

        activeStone = starStoneActive.Orange;
        orangedisplay.text = "Active";

      
    }

    public void activateBlue()
    {
        //
        // Changes active state to Blue
        // Updates UI, if game isn't currently started it begins on stone selection
        //

        icons.alpha = 1;

        if (activeStone == starStoneActive.None)
        {
            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().startGame();
        }

        activeStone = starStoneActive.Blue;
        bluedisplay.text = "Active";

        
    }

    public void activatePink()
    {
        //
        // Changes active state to Pink
        // Updates UI, if game isn't currently started it begins on stone selection
        //

        icons.alpha = 1;

        if (activeStone == starStoneActive.None)
        {
            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().startGame();
        }

        activeStone = starStoneActive.Pink;
        pinkdisplay.text = "Active";

       
    }

    public void activateNone()
    {
        //
        // Sets state to "None", meaning no stones are active (DEFAULT STATE)
        //

        icons.alpha = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<MeshRenderer>().material = normal;
        }
    }
}
