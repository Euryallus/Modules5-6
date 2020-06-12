using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Set in inspector:
    [SerializeField]
    private GameObject prefabEnemyHitPopup;

    private void Awake()
    {
        //Ensure that an instance of the UIManager class does not already exist
        if (instance == null)
        {
            //Set this class as the instance and ensure that it stays when changing scenes
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //If there is an existing instance that is not this, destroy the GameObject this script is connected to
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ShowEnemyHitPopup(int hitPoints, Vector3 enemyPosition)
    {
        if(Camera.main != null)
        {
            Transform transformCanvas = GameObject.Find("Canvas").transform;

            Vector2 popupPos = Camera.main.WorldToScreenPoint(enemyPosition);
            GameObject goPopup = Instantiate(prefabEnemyHitPopup, popupPos, Quaternion.identity, transformCanvas);
            goPopup.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "<b>HIT</b>\n-" + hitPoints;
        }
    }
}
