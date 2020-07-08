using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

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

    private int wavePointer = 0;
    
    private bool nextWaveStarted = false;

    private float waveTimer;
    private float waveLength;

    private GameObject[] remainingEnemies;
    private GameObject[] spawners;
    private GameObject player;

    public float timeBeforeGameStarts = 5f;


    protected enum waveState
    {
        beforeWaveStart, waveActive, waveFail, waveComplete, gameWon, gameLost
    }

    protected waveState current;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(autoStart());
        
    }
    public void startGame() //begins game from the top
    {
        wavePointer = 0;
        initiateWave(waves[0]);

       //while(doors.Count < waves.Count + 1)
       //{
       //    doors.Insert(doors.Count, null);
       //}

        //for (int i = 0; i < waves.Count; i++)
        //{
        //    if(doors.Count < waves.Count)
        //    {
        //        if(doors[i] == null)
        //        {
        //            doors.Insert(i, null);
        //        }
        //    }
        //}
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

                waveDisplay.text = "Wave " + (wavePointer + 1).ToString() + " of " + waves.Count.ToString();
                break;

            case waveState.waveFail:
                timeRemaining.text = "Wave failed!";
                waveDisplay.text = "";
                break;

            case waveState.waveComplete:
                //
                // ## checks if this is the last wave - if not, waits for DownTime until starting the next
                // ## if none come next, game is complete - if generator is fixed, the game is won
                // ## if generator is still broken, game is lost
                //

                timeRemaining.text = "Wave complete!";

                if(nextWaveStarted == false)
                {
                    if(doors[wavePointer] != null)
                    {
                        doors[wavePointer].SetLocked(false);
                        doors[wavePointer].transform.GetChild(0).GetChild(0).gameObject.layer = 10;
                        doors[wavePointer].transform.GetChild(0).GetChild(1).gameObject.layer = 10;

                        GameObject.FindGameObjectWithTag("navMesh").GetComponent<NavMeshSurface>().BuildNavMesh();

                        Debug.Log("REBUILT");
                    }
                    StartCoroutine(waitForNextWave(waves[wavePointer].downtime));
                    nextWaveStarted = true;
                }
                waveDisplay.text = "";

                break;

            case waveState.gameWon:
                timeRemaining.text = "Game won!";
                waveDisplay.text = "";
                break;

            case waveState.gameLost:
                timeRemaining.text = "FAILED";
                waveDisplay.text = "";
                break;
        }

        Debug.Log("MOVED TO " + player.transform.position);
    }

    public void checkRemainingEnemies()
    {
        //
        // ## Checks how many enemies are present in scene & sets state to waveComplete depending
        //

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
            if(GameObject.FindGameObjectWithTag("GeneratorManager").GetComponent<GeneratorRepair>().GetGeneratorRepaired() == true)
            {
                current = waveState.gameWon;
            }
            else
            {
                current = waveState.gameLost;
            }
            
        }
        else //waits for 1/2 downtime, changes state to beforeWave
        {   // waits another 1/2 downtime before starting wave
            yield return new WaitForSeconds(waitLength / 2);
            current = waveState.beforeWaveStart;
            yield return new WaitForSeconds(waitLength / 2);

            if(wavePointer == waves.Count - 1 && waves[wavePointer].bossNumbers > 0)
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
