using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class generatorStates : MonoBehaviour
{
    GameObject[] enemies;

    public enum starStoneActive
    {
        None,
        Purple,
        Orange,
        Blue,
        Pink
    }

    [SerializeField]
    private starStoneActive activeStone = starStoneActive.None;

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

    [SerializeField]
    float stoneActiveTime = 6f;

    float BlueActive = 0;
    float OrangeActive = 0;
    float PinkActive = 0;
    float PurpleActive = 0;

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

    [SerializeField]
    float rechargeDelay = 4;

    public starStoneActive returnState()
    {
        return activeStone;
    }

    public void FixedUpdate()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        switch (activeStone)
        {
            case starStoneActive.Blue:
                BlueActive += Time.deltaTime;

                if(BlueActive > stoneActiveTime)
                {
                    activateOrange();
                }

                break;

            case starStoneActive.Orange:
                OrangeActive += Time.deltaTime;

                if (OrangeActive > stoneActiveTime)
                {
                    activatePink();
                }

                break;

            case starStoneActive.Pink:
                PinkActive += Time.deltaTime;

                if (PinkActive > stoneActiveTime)
                {
                    activatePurple();

                }

                break;

            case starStoneActive.Purple:

                PurpleActive += Time.deltaTime;

                if (PurpleActive > stoneActiveTime)
                {
                    activateBlue();
                }

                break;

            case starStoneActive.None:
                icons.alpha = 0;
                break;
        }

        //recharge

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
        icons.alpha = 1;

        if(activeStone == starStoneActive.None)
        {
            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().startGame();
        }

        activeStone = starStoneActive.Purple;
        purpledisplay.text = "Active";
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<MeshRenderer>().material = purple;
        }
    }

    public void activateOrange()
    {
        icons.alpha = 1;

        if (activeStone == starStoneActive.None)
        {
            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().startGame();
        }

        activeStone = starStoneActive.Orange;
        orangedisplay.text = "Active";

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<MeshRenderer>().material = orange;
        }
    }

    public void activateBlue()
    {
        icons.alpha = 1;

        if (activeStone == starStoneActive.None)
        {
            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().startGame();
        }

        activeStone = starStoneActive.Blue;
        bluedisplay.text = "Active";

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<MeshRenderer>().material = blue;
        }
    }

    public void activatePink()
    {
        icons.alpha = 1;

        if (activeStone == starStoneActive.None)
        {
            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().startGame();
        }

        activeStone = starStoneActive.Pink;
        pinkdisplay.text = "Active";

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<MeshRenderer>().material = pink;
        }
    }

    public void activateNone()
    {
        icons.alpha = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<MeshRenderer>().material = normal;
        }
    }
}
