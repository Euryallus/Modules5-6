using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//------------------------------------------------------\\
//  Shows game controls, then loads the MAP scene       \\  
//  asnychronously while displaying loading text        \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class MapLoadScene : MonoBehaviour
{
    //Set in inspector:
    public TextMeshProUGUI textLoading; //The UI text used to display info/load percentage
    public GameObject goLoadButton;     //The button that triggers the loading of the MAP scene
    public GameObject goLoadIndicator;  //The animated loading indicator

    private bool loading;       //Used to check if the next scene is currently being loaded
    private float timeInScene;  //Used to keep track of the amount of time since the MapLoadScene was loaded

    // Start is called before the first frame update
    void Start()
    {
        //Reset scene to default state
        timeInScene = 0f;
        goLoadButton.SetActive(true);
        goLoadIndicator.SetActive(false);

        //Set starting text so the player knows to look at the controls
        textLoading.text = "Please familiarise yourself with the controls.";
    }

    private void Update()
    {
        //Increase time in scene by deltaTime so it counts up in realtime
        timeInScene += Time.unscaledDeltaTime;

        //Player can press space to start loading as an alternative to clicking the button
        //  but only after being in the scene for 2 seconds, to prevent accidental skipping of the scene
        if (Input.GetKeyDown(KeyCode.Space) && timeInScene > 2f)
        {
            ButtonReadyToLoad();
        }
    }

    private IEnumerator LoadMapAsync()
    {
        //Start loading, reset load text
        loading = true;
        textLoading.text = "Loading (0%)";

        //Wait for a short amount of time so the loading screen
        //  does not flash in/out too quickly on fast computers
        yield return new WaitForSeconds(1.5f);

        //Start the asnychronous loading operation
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("MAP");

        //While loading, update text to show load percentage
        while (!loadOperation.isDone)
        {
            textLoading.text = "Loading (" + Mathf.RoundToInt(loadOperation.progress / 0.01f) + "%)";
            yield return null;
        }
        yield return null;
    }

    //Starts loading the MAP scene on button load
    public void ButtonReadyToLoad()
    {
        if (!loading)
        {
            StartCoroutine(LoadMapAsync());
            goLoadButton.SetActive(false);
            goLoadIndicator.SetActive(true);
        }
    }
}
