using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Set in inspector:
    [SerializeField]
    private GameObject prefabEnemyHitPopup;

    private GameObject goCanvas;
    private Slider sliderAttackInterval;
    private WeaponHolder weaponHolderPlayer;

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

    private void Start()
    {
        goCanvas = GameObject.Find("Canvas");
        sliderAttackInterval = goCanvas.transform.Find("Attack Interval Slider").GetComponent<Slider>();
        weaponHolderPlayer = GameObject.Find("Player").GetComponent<WeaponHolder>();
    }

    private void Update()
    {
        Weapon activePlayerWeapon = weaponHolderPlayer.activeWeapon;
        if (activePlayerWeapon.m_attackIntervalTimer > 0f && activePlayerWeapon.m_template.GetAttackInterval() > 0.1f)
        {
            sliderAttackInterval.gameObject.SetActive(true);
            float attackTimerPerc = activePlayerWeapon.m_attackIntervalTimer / activePlayerWeapon.m_template.GetAttackInterval();
            sliderAttackInterval.value = attackTimerPerc;
        }
        else
        {
            sliderAttackInterval.gameObject.SetActive(false);
        }
    }

    public void ShowEnemyHitPopup(int hitPoints, Vector3 enemyPosition)
    {
        if(Camera.main != null)
        {
            Vector2 popupPos = Camera.main.WorldToScreenPoint(enemyPosition);
            GameObject goPopup = Instantiate(prefabEnemyHitPopup, popupPos, Quaternion.identity, goCanvas.transform);
            goPopup.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "<b>HIT</b>\n-" + hitPoints;
        }
    }
}
