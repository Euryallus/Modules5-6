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
    [SerializeField]
    private GameObject prefabDoorLockedPopup;

    private GameObject goCanvas;
    private Slider sliderAttackInterval;
    private Slider sliderWeaponCharge;
    private Slider sliderPrototypeCharge;
    private WeaponHolder weaponHolderPlayer;

    private GameObject doorLockedPopup;
    private Vector3 doorLockedPopupPosition;

    private void Awake()
    {
        //Ensure that an instance of the UIManager class does not already exist
        if (instance == null)
        {
            //Set this class as the static instance so it can easily be accessed from any script
            instance = this;
            //DontDestroyOnLoad(gameObject);
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
        sliderWeaponCharge = goCanvas.transform.Find("Weapon Charge Slider").GetComponent<Slider>();
        sliderPrototypeCharge = goCanvas.transform.Find("Prototype Charge Slider").GetComponent<Slider>();
        weaponHolderPlayer = GameObject.Find("Player").GetComponent<WeaponHolder>();
    }

    private void Update()
    {
        Weapon activePlayerWeapon = weaponHolderPlayer.activeWeapon;
        if(activePlayerWeapon != null)
        {
            UpdateAttackIntervalSlider(activePlayerWeapon);
            UpdateWeaponChargeSlider(activePlayerWeapon);
            UpdatePrototypeChargeSlider();
        }

        if (doorLockedPopup != null)
        {
            doorLockedPopup.transform.position = Camera.main.WorldToScreenPoint(doorLockedPopupPosition);
        }
    }

    public void ShowDoorLockedPopup(Vector3 position)
    {
        if(doorLockedPopup == null)
        {
            doorLockedPopupPosition = position;
            doorLockedPopup = Instantiate(prefabDoorLockedPopup, goCanvas.transform);
        }
    }
    public void HideDoorLockedPopup()
    {
        if(doorLockedPopup != null)
        {
            Destroy(doorLockedPopup);
        }
    }

    private void UpdatePrototypeChargeSlider()
    {
        PrototypeWeapon protoWeapon = weaponHolderPlayer.GetPrototypeWeapon();
        if(sliderPrototypeCharge != null && protoWeapon != null)
        {
            sliderPrototypeCharge.value = protoWeapon.m_charge / protoWeapon.m_prototypeTemplate.GetMaxCharge();
        }
    }

    private void UpdateAttackIntervalSlider(Weapon activePlayerWeapon)
    {
        if(sliderAttackInterval != null)
        {
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
    }

    private void UpdateWeaponChargeSlider(Weapon activePlayerWeapon)
    {
        if(sliderWeaponCharge != null)
        {
            if (activePlayerWeapon is PrototypeWeapon activePlayerProto)
            {
                if (activePlayerProto.m_poweringUp)
                {
                    sliderWeaponCharge.gameObject.SetActive(true);
                    sliderWeaponCharge.value = activePlayerProto.m_damagePower / 1f;
                }
                else
                {
                    sliderWeaponCharge.gameObject.SetActive(false);
                }
            }
            else
            {
                sliderWeaponCharge.gameObject.SetActive(false);
            }
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
