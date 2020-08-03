using UnityEngine;
using TMPro;
using UnityEngine.UI;

//------------------------------------------------------\\
//  Handles UI elements in the main game scene          \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//      and modified/optimised for prototype phase      \\
//------------------------------------------------------\\

public class UIManager : MonoBehaviour
{
    //A single static instance of this class always exists
    //  so it can easily be used by other scripts at any time
    public static UIManager instance;

    //Set in inspector:
    [SerializeField]
    private GameObject prefabEnemyHitPopup;     //Prefab for the popup shown when an enemy is hit/hurt
    [SerializeField]
    private GameObject prefabDoorLockedPopup;   //Prefab for the popup shown to tell the player a door is locked

    private GameObject goCanvas;                //The main canvas that contains all UI elements
    private Slider sliderAttackInterval;        //Slider used to show time until the player can use a weapon
    private Slider sliderWeaponCharge;          //Slider that shows how much damage a weapon will do
    private Slider sliderPrototypeCharge;       //Slider that shows how much charge is remaining for the prototype weapon
    private WeaponHolder weaponHolderPlayer;    //The WeaponHolder script that contains all player weapons
    private GameObject doorLockedPopup;         //Instantiated popup that tells the player when door is locked
    private Vector3 doorLockedPopupPosition;    //Determines where on the screen the door locked popup should be shown

    private void Awake()
    {
        //Ensure that an instance of the UIManager class does not already exist
        if (instance == null)
        {
            //Set this class as the static instance so it can easily be accessed from any script
            instance = this;
        }
        //If there is an existing instance that is not this, destroy the GameObject this script is connected to
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Find UI GameObjects and the player's weaponHolder in the scene
        goCanvas = GameObject.Find("Canvas");
        sliderAttackInterval = goCanvas.transform.Find("Attack Interval Slider").GetComponent<Slider>();
        sliderWeaponCharge = goCanvas.transform.Find("Weapon Charge Slider").GetComponent<Slider>();
        sliderPrototypeCharge = goCanvas.transform.Find("Weapon Info").Find("Prototype Charge Slider").GetComponent<Slider>();
        weaponHolderPlayer = GameObject.Find("Player").GetComponent<WeaponHolder>();
    }

    private void Update()
    {
        //Update UI elements based on the player's held weapon
        Weapon activePlayerWeapon = weaponHolderPlayer.activeWeapon;
        if(activePlayerWeapon != null)
        {
            UpdateAttackIntervalSlider(activePlayerWeapon);
            UpdateWeaponChargeSlider(activePlayerWeapon);
            UpdatePrototypeChargeSlider();
        }

        //If showing a door locked popup, update its position
        if (doorLockedPopup != null)
        {
            Vector3 popupScreenPos = Camera.main.WorldToScreenPoint(doorLockedPopupPosition);
            if (popupScreenPos.z > 0f)
            {
                //If popupScreenPos.z > 0, the player is facing the door, so position the popup on screen based on the door's position
                doorLockedPopup.transform.position = Camera.main.WorldToScreenPoint(doorLockedPopupPosition);
            }
            else
            {
                //If popupScreenPos.z <= 0, the player is facing away from the door, so hide the popup off-screen
                doorLockedPopup.transform.position = new Vector3(-100f, -100f, 0f);
            }
        }
    }

    //Showing/hiding the popup that tells the player if a door is locked
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

    //Set the charge slider value based on the prototype weapon charge
    private void UpdatePrototypeChargeSlider()
    {
        PrototypeWeapon protoWeapon = weaponHolderPlayer.GetPrototypeWeapon();
        if(sliderPrototypeCharge != null && protoWeapon != null)
        {
            //Get percentage charge by dividing current charge by max charge
            sliderPrototypeCharge.value = protoWeapon.m_charge / protoWeapon.m_prototypeTemplate.GetMaxCharge();
        }
    }

    private void UpdateAttackIntervalSlider(Weapon activePlayerWeapon)
    {
        if(sliderAttackInterval != null)
        {
            //Only show the attack interval slider if the current weapon has an attack interval,
            //  and the attack interval is not insignificantly small to avoid the slider showing for a very short amount of time
            if (activePlayerWeapon.m_attackIntervalTimer > 0f && activePlayerWeapon.m_template.GetAttackInterval() > 0.1f)
            {
                sliderAttackInterval.gameObject.SetActive(true);
                //Get percentage charge by dividing current attack interval by total attack interval
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
            //Only show the weapon charge slider if the player is holding
            //  a prototype weapon that is powering up
            if (activePlayerWeapon is PrototypeWeapon activePlayerProto)
            {
                if (activePlayerProto.m_poweringUp)
                {
                    sliderWeaponCharge.gameObject.SetActive(true);
                    //Set slider to a percentage value based on the weapon's damage power
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
            //Show a popup that displays how much health was taken from an enemy
            Vector2 popupPos = Camera.main.WorldToScreenPoint(enemyPosition);
            GameObject goPopup = Instantiate(prefabEnemyHitPopup, popupPos, Quaternion.identity, goCanvas.transform);
            goPopup.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = hitPoints.ToString();
        }
    }
}
