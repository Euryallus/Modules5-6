using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    activateNone();    
                }

                break;
        }
    }

    public void activatePurple()
    {
        activeStone = starStoneActive.Purple;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<MeshRenderer>().material = purple;
        }
    }

    public void activateOrange()
    {
        activeStone = starStoneActive.Orange;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<MeshRenderer>().material = orange;
        }
    }

    public void activateBlue()
    {
        activeStone = starStoneActive.Blue;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<MeshRenderer>().material = blue;
        }
    }

    public void activatePink()
    {
        activeStone = starStoneActive.Pink;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<MeshRenderer>().material = pink;
        }
    }

    public void activateNone()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<MeshRenderer>().material = normal;
        }
    }




}
