using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//------------------------------------------------------\\
//  Loads the MAP scene asnychronously while            \\  
//  displaying loading text                             \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class MapLoadScene : MonoBehaviour
{
    //Set in inspector:
    public TextMeshProUGUI textLoading; //The UI text used to display load percentage

    // Start is called before the first frame update
    void Start()
    {
        //Start loading/reset load text
        textLoading.text = "Please familiarise yourself with the controls.";
    }

    private IEnumerator LoadMapAsync()
    {
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

    public void ButtonReadyToLoad()
    {
        StartCoroutine(LoadMapAsync());
    }
}
