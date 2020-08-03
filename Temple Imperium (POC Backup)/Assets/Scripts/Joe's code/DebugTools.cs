using UnityEngine;
using UnityEngine.UI;
using TMPro;

//------------------------------------------------------\\
//  DebugTools was used for the POC phase to easily     \\
//  perform certain actions through debug shortcuts     \\
//  and show debug info in the UI.                      \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class DebugTools : MonoBehaviour
{
    //Set in inspector:
    public Camera cameraOverview;           //Camera for viewing the scene from above
    public Camera cameraPlayer;             //First person player camera
    public Image imgCrosshair;              //Crosshair for first person mode
    public TextMeshProUGUI textWeaponName;  //Text showing the current weapon being held
    public TextMeshProUGUI textInfo1;       //Text for displaying debug info
    public TextMeshProUGUI textInfo2;       //Second line of text for displaying debug info

    // Start is called before the first frame update
    void Start()
    {
        //Switch to the overview camera by default
        ButtonSwitchCamera(false);
    }

    // Update is called once per frame
    void Update()
    {
        WeaponHolder weaponHolder = GameObject.Find("Player").GetComponent<WeaponHolder>();
        if(weaponHolder != null && weaponHolder.activeWeapon != null)
        {
            //If a weapon is being held, display its name
            textWeaponName.text = weaponHolder.activeWeapon.m_template.GetWeaponName();
            if (weaponHolder.activeWeapon is GunWeapon gun)
            {
                //Extra info to display for guns: reloading state and ammo
                if (gun.m_reloading)
                {
                    textWeaponName.text += " (Reloading)";
                }
                textInfo1.gameObject.SetActive(true);
                textInfo2.gameObject.SetActive(true);
                textInfo1.text = "Ammo: " + gun.m_totalAmmo;
                textInfo2.text = "Loaded ammo: " + gun.m_loadedAmmo + "/" + gun.m_gunTemplate.GetMagazineSize();
            }
            else if(weaponHolder.activeWeapon is GrenadeWeapon grenade)
            {
                //Extra info to display for grenades: number remaining and shortcut info
                textInfo1.gameObject.SetActive(true);
                textInfo2.gameObject.SetActive(true);
                textInfo1.text = "Grenades remaining: " + grenade.m_grenadeCount;
                textInfo2.text = "(Press G to add a grenade)";
            }
            else
            {
                //Hide extra info when it is not needed
                textInfo1.gameObject.SetActive(false);
                textInfo2.gameObject.SetActive(false);
            }
        }

        //Shortcuts for switching between first person/overview cameras
        if (Input.GetKeyDown(KeyCode.M))
        {
            ButtonSwitchCamera(false);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            ButtonSwitchCamera(true);
        }
    }

    public void ButtonSwitchCamera(bool player)
    {
        //Can't switch cameras if one or both are not in the scene
        if (cameraPlayer == null || cameraOverview == null)
            return;

        //Enable either the first person or overview camera, as well as their audio listners so audio
        //  sounds like it is coming from a relative location
        cameraPlayer.enabled = player;
        cameraPlayer.gameObject.GetComponent<AudioListener>().enabled = player;
        cameraOverview.enabled = !player;
        cameraOverview.gameObject.GetComponent<AudioListener>().enabled = !player;

        //Lock and hide the cursor only if in first person
        Cursor.lockState = player ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = !player;
        //Show the crosshair only if in first person
        imgCrosshair.enabled = player;
    }
}
