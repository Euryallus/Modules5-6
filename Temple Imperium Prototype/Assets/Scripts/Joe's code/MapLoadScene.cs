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
    public GameObject goFadeCover;      //The black cover that fades in as a scene transition

    private bool loading;       //Used to check if the next scene is currently being loaded
    private float timeInScene;  //Used to keep track of the amount of time since the MapLoadScene was loaded

    // Start is called before the first frame update
    void Start()
    {
        //Reset scene to default state
        timeInScene = 0f;
        goLoadButton.SetActive(true);
        goLoadIndicator.SetActive(false);
        goFadeCover.SetActive(false);

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
        //Disable scene activation so the scene does not change as soon as loding is complete,
        //  to allow a fade transition first
        loadOperation.allowSceneActivation = false;

        //While loading, update text to show load percentage
        while (loadOperation.progress < 0.9f)
        {
            textLoading.text = "Loading (" + Mathf.RoundToInt(loadOperation.progress / 0.01f) + "%)";
            yield return null;
        }

        //Scene activation is done, show that loading is complete and star the fade transition
        textLoading.text = "Loading (100%)";
        goFadeCover.SetActive(true);

        //Wait for the fade transition animation to finish, then switch to the MAP scene
        yield return new WaitForSeconds(1.1f);
        loadOperation.allowSceneActivation = true;
        Cursor.visible = false;
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
