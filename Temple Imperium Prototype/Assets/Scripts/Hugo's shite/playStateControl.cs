﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Manages wave UI and handles wave state (before, during, won, failed, etc.)
//

public class playStateControl : MonoBehaviour
{
    [Header("Wave info")]
        [SerializeField]
        List<waveData> waves = new List<waveData>();
        [SerializeField]
        List<Door> doors = new List<Door>();
        [SerializeField]
        private Text timeRemaining;
        [SerializeField]
        private Transform bossWavePlayerSpawnLocation;
        [SerializeField]
        private Transform bossWaveBossSpawnLocation;
        [SerializeField]
        private Text waveDisplay;
        [SerializeField]
        private GameObject boss;
    [SerializeField]
    private GameObject waveBossTemplate;
    [SerializeField]
    private int waveBossEveryXWaves = 5;

    private int wavePointer = 0;
    
    private bool nextWaveStarted = false;

    private float waveTimer;
    private float waveLength;

    private GameObject[] remainingEnemies;
    private GameObject[] spawners;
    private GameObject player;

    private GameObject failMenu;

    public float timeBeforeGameStarts = 5f;

    private bool isEndlessMode = false;

    [Header("Horde mode alterations")]
        [Header("Round 0 to 5 Enemy 1 values")]
        [SerializeField]
        private int Enemy1Min1 = 2;
        private int Enemy1Max1 = 5;

        [Header("Round 6 to 10 Enemy 1 & 2 values")]
        [SerializeField]
        private int Enemy1Min2 = 3;
        private int Enemy1Max2 = 6;

        private int Enemy2Min2 = 1;
        private int Enemy2Max2 = 3;

        [Header("Round 11 onwards Enemy values")]
        [SerializeField]
        private int Enemy1Min3 = 3;
        private int Enemy1Max3 = 6;

        private int Enemy2Min3 = 2;
        private int Enemy2Max3 = 4;

        private int Enemy3Min3 = 1;
        private int Enemy3Max3 = 2;

    [Header("UI stuff")]
    [SerializeField]
    CanvasGroup gameWonFade;


    public enum waveState
    {
        beforeWaveStart, waveActive, waveFail, waveComplete, gameWon, gameLost
    }

    protected waveState current;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        failMenu = GameObject.FindGameObjectWithTag("failMenu");
        failMenu.SetActive(false);
        StartCoroutine(autoStart());

        if(PlayerPrefs.GetInt("EndlessMode", 0) == 1)
        {
            isEndlessMode = true;

            GameObject[] doorTemp = GameObject.FindGameObjectsWithTag("Door");
            for (int i = 0; i < doorTemp.Length; i++)
            {
                doorTemp[i].GetComponent<Door>().SetLocked(false);
            }

        }
        else
        {
            isEndlessMode = false;
        }
        
    }
    public void startGame() //begins game from the top
    {
        wavePointer = 0;
        if (isEndlessMode)
        {
            waveData startWave = new waveData(0, 0.5f, Random.Range(3, 6), 0, 0, 90, 15);
            initiateWave(startWave);
        }
        else
        {
            initiateWave(waves[0]);
        }
    }

    public waveState returnState()
    {
        return current;
    }

    private IEnumerator autoStart()
    {
        yield return new WaitForSeconds(timeBeforeGameStarts);
        GameObject.FindGameObjectWithTag("GeneratorManager").GetComponent<starStoneManager>().activateStone((starStoneManager.starStones)Random.Range(0, 4));
        startGame();
    }

    public void playerDied()
    {
        current = waveState.gameLost;
    }

    public void initiateWave(waveData wave) 
    {
        //
        // ## Calls spawn script from Spawners in scene
        // ## Sets current state to waveActive
        //

        waveLength = wave.waveLength;
        waveTimer = 0;
        spawners = GameObject.FindGameObjectsWithTag("Spawner");

        if(wavePointer % waveBossEveryXWaves == 0 && isEndlessMode && wavePointer != 0)
        {
            for (int i = 0; i < wavePointer / waveBossEveryXWaves; i++)
            {
                GameObject waveBoss = Instantiate(waveBossTemplate);
                waveBoss.transform.position = spawners[Random.Range(0, spawners.Length)].transform.position;
            }
            
        }
        else
        {
            for (int i = 0; i < spawners.Length; i++)
            {
                if (isEndlessMode)
                {
                    spawners[i].GetComponent<spawnerScript>().startWave(wave);
                }
                else
                {
                    spawners[i].GetComponent<spawnerScript>().startWave(waves[wavePointer]);
                }
            
            }
        }

        

        current = waveState.waveActive;

    }       

    private void FixedUpdate()
    {
        switch (current)
        {
            case waveState.beforeWaveStart:
                timeRemaining.text = "Get ready";
                waveDisplay.text = "";
                break;

            case waveState.waveActive:
                //
                // ## Updates wave UI timer, calculates if time has run out & changes state accordingly
                //

                waveTimer += Time.deltaTime;

                if(waveTimer > waveLength)
                {
                    current = waveState.waveFail;
                }
                else
                {
                    checkRemainingEnemies();

                    float remaining = waveLength - waveTimer;
                    string minutes = Mathf.Floor(remaining / 60).ToString("00");
                    string seconds = (remaining % 60).ToString("00");

                    timeRemaining.text = minutes + ":" + seconds;
                }
                if (isEndlessMode)
                {
                    waveDisplay.text = "Wave " + (wavePointer + 1).ToString() + " of ??";
                }
                else
                {
                    waveDisplay.text = "Wave " + (wavePointer + 1).ToString() + " of " + waves.Count.ToString();
                }
                
                break;

            case waveState.waveFail:
                timeRemaining.text = "";
                player.GetComponent<playerHealth>().stopMovement();

                failMenu.SetActive(true);
                failMenu.GetComponent<Animator>().Play("deadFade", 0);
                waveDisplay.text = "";
                break;

            case waveState.waveComplete:
                //
                // ## checks if this is the last wave - if not, waits for DownTime until starting the next
                // ## if none come next, game is complete - if generator is fixed, the game is won
                // ## if generator is still broken, game is lost
                //
                if (wavePointer == waves.Count && isEndlessMode == false)
                {
                    current = waveState.gameWon;
                }

                timeRemaining.text = "Wave complete!";

                if(nextWaveStarted == false)
                {
                    if(isEndlessMode == false)
                    {
                        if(doors[wavePointer] != null)
                        {
                            doors[wavePointer].SetLocked(false);
                            doors[wavePointer].transform.GetChild(0).GetChild(0).gameObject.layer = 10;
                            doors[wavePointer].transform.GetChild(0).GetChild(1).gameObject.layer = 10;

                            GameObject.FindGameObjectWithTag("navMesh").GetComponent<NavMeshSurface>().BuildNavMesh();
                        }
                        StartCoroutine(waitForNextWave(waves[wavePointer].downtime));
                    }
                    else
                    {
                        StartCoroutine(waitForNextWave(20));
                    }
                    nextWaveStarted = true;
                }
                waveDisplay.text = "";

                break;

            case waveState.gameWon:
                timeRemaining.text = "";
                player.GetComponent<playerHealth>().stopMovement();
                if (gameWonFade.alpha < 1)
                {
                    gameWonFade.alpha += 0.5f * Time.deltaTime;
                    if(gameWonFade.alpha >= 1)
                    {
                        TextCutscene.storyIndex = 0;

                        SceneManager.LoadScene("EndScene");

                    }
                }
                waveDisplay.text = "";
                break;

            case waveState.gameLost:
                timeRemaining.text = "FAILED";
                waveDisplay.text = "";
                break;
        }
    }


    public void checkRemainingEnemies()
    {
        //
        // ## Checks how many enemies are present in scene & sets state to waveComplete depending
        //

        if(waveTimer > 3f)
        {
            remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            if(remainingEnemies.Length == 0)
            {
                current = waveState.waveComplete;
            }
        }
    }

    private IEnumerator waitForNextWave(float waitLength)
    {
        wavePointer += 1;

        if (isEndlessMode)
        {
            yield return new WaitForSeconds(waitLength / 2);
            current = waveState.beforeWaveStart;
            yield return new WaitForSeconds(waitLength / 2);

            int enemy1 = 1;
            int enemy2 = 0;
            int enemy3 = 0;
            float waveTime = 90;


            if(wavePointer > 0 && wavePointer < 6)
            {
                enemy1 = Random.Range(Enemy1Min1, Enemy1Max1 + 1);
                enemy2 = 0;
                enemy3 = 0;
            }
            else if(wavePointer > 5 && wavePointer < 11)
            {
                enemy1 = Random.Range(Enemy1Min2, Enemy1Max2 + 1);
                enemy2 = Random.Range(Enemy2Min2, Enemy2Max2 + 1);
                enemy3 = 0;
            }
            else
            {
                int difficultyMod = ((wavePointer - 11) / 5);
                if(difficultyMod == 0)
                {
                    difficultyMod = 1;
                }
                enemy1 = Random.Range(Enemy1Min3, Enemy1Max3 + 1) * difficultyMod;
                enemy2 = Random.Range(Enemy2Min3, Enemy2Max3 + 1) * difficultyMod;
                enemy3 = Random.Range(Enemy3Min3, Enemy3Max3 + 1) * difficultyMod;
            }

            waveTime = (enemy1 + enemy2 + enemy3) * 10;
            waveData newWave = new waveData(wavePointer, 0.5f, enemy1, enemy2, enemy3, waveTime, 20);
            initiateWave(newWave);

            nextWaveStarted = false;
        }
        else
        {
            if(wavePointer == waves.Count - 1 && GameObject.FindGameObjectWithTag("GeneratorManager").GetComponent<GeneratorRepair>().GetGeneratorRepaired() == false)
            {
                    current = waveState.waveFail;
            }
            else
            {
                yield return new WaitForSeconds(waitLength / 2);
                current = waveState.beforeWaveStart;
                yield return new WaitForSeconds(waitLength / 2);

                if (waves[wavePointer].bossNumbers > 0)
                {
                    initiateBossFight();
                }
                else
                {
                    initiateWave(waves[wavePointer]);
                }
                
                nextWaveStarted = false;
            }
        } 
    }

    private void initiateBossFight()
    {
        player.GetComponent<playerMovement>().enabled = false;
        player.transform.position = bossWavePlayerSpawnLocation.position;
        player.GetComponent<playerMovement>().enabled = true;

        waveLength = waves[wavePointer].waveLength;
        waveTimer = 0;


        boss.SetActive(true);
        //
        current = waveState.waveActive;
        nextWaveStarted = true;
        //current = waveState.waveActive;
    }
}
