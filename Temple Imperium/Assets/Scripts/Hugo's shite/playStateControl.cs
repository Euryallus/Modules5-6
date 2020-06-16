using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playStateControl : MonoBehaviour
{

    float waveTimer;
    float waveLength;

    GameObject[] remainingEnemies;
    public Text timeRemaining;

    public spawnerScript spawner;

    private void Start()
    {

        spawner = gameObject.GetComponent<spawnerScript>();
    }

    protected enum waveState
    {
        beforeWaveStart, waveActive, waveFail, waveComplete
    }

    protected waveState current;

   public void initiateWave(float waveTimeInSeconds)
   {
        waveLength = waveTimeInSeconds;
        waveTimer = 0;
        current = waveState.waveActive;
        
   }       

    private void FixedUpdate()
    {
        switch (current)
        {
            case waveState.beforeWaveStart:
                timeRemaining.text = "Get ready...";
                break;

            case waveState.waveActive:

                waveTimer += Time.deltaTime;

                if(waveTimer > waveLength)
                {
                    current = waveState.waveFail;
                }
                else
                {
                    if(spawner.spawning == false)
                    {
                        checkRemainingEnemies();
                    }
                    
                    timeRemaining.text = ("Time remaining: " + (waveLength - waveTimer).ToString("F0"));
                }

                

                break;

            case waveState.waveFail:
                timeRemaining.text = "Wave failed!";
                break;

            case waveState.waveComplete:
                timeRemaining.text = "Wave complete!";
                break;
        }
    }

    public void checkRemainingEnemies()
    {
        remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if(remainingEnemies.Length == 0)
        {
            current = waveState.waveComplete;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            for (int i = 0; i < remainingEnemies.Length; i++)
            {
                Destroy(remainingEnemies[i]);
            }
        }
    }
}
