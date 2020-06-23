using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playStateControl : MonoBehaviour
{
    [SerializeField]
    List<waveData> waves = new List<waveData>();

    [SerializeField]
    List<Door> doors = new List<Door>();

    [SerializeField]
    int wavePointer = 0;
    GameObject[] spawners;

    float waveTimer;
    float waveLength;

    GameObject[] remainingEnemies;
    public Text timeRemaining;

    bool nextWaveStarted = false;

    protected enum waveState
    {
        beforeWaveStart, waveActive, waveFail, waveComplete, gameWon
    }

    protected waveState current;


    public void startGame()
    {
        wavePointer = 0;
        initiateWave(waves[0]);
    }

   public void initiateWave(waveData wave)
   {
        waveLength = wave.waveLength;
        waveTimer = 0;
        spawners = GameObject.FindGameObjectsWithTag("Spawner");

        for (int i = 0; i < spawners.Length; i++)
        {
            spawners[i].GetComponent<spawnerScript>().startWave(waves[wavePointer]);
        }

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
                    checkRemainingEnemies();
                    timeRemaining.text = ("Time remaining: " + (waveLength - waveTimer).ToString("F0"));
                }
                break;

            case waveState.waveFail:
                timeRemaining.text = "Wave failed!";
                break;

            case waveState.waveComplete:
                timeRemaining.text = "Wave complete!";
                if(nextWaveStarted == false)
                {
                    if(doors[wavePointer] != null)
                    {
                        doors[wavePointer].SetLocked(false);
                    }
                    StartCoroutine(waitForNextWave(waves[wavePointer].downtime));
                    nextWaveStarted = true;
                }

                break;

            case waveState.gameWon:
                timeRemaining.text = "Game won!";
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
    }

    private IEnumerator waitForNextWave(float waitLength)
    {
        wavePointer += 1;

        if (wavePointer == waves.Count)
        {
            current = waveState.gameWon;
        }
        else
        {
            yield return new WaitForSeconds(waitLength / 2);
            current = waveState.beforeWaveStart;
            yield return new WaitForSeconds(waitLength / 2);
        
            initiateWave(waves[wavePointer]);

            nextWaveStarted = false;
        }
        
    }
}
