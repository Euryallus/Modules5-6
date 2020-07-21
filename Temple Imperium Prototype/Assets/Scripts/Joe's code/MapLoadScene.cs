using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MapLoadScene : MonoBehaviour
{
    public TextMeshProUGUI textLoading;

    // Start is called before the first frame update
    void Start()
    {
        textLoading.text = "Loading (0%)";
        StartCoroutine(LoadMapAsync());
    }

    private IEnumerator LoadMapAsync()
    {
        yield return new WaitForSeconds(1.5f);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("MAP");

        while (!loadOperation.isDone)
        {
            textLoading.text = "Loading (" + Mathf.RoundToInt(loadOperation.progress / 0.01f) + "%)";
            yield return null;
        }
        yield return null;
    }
}
