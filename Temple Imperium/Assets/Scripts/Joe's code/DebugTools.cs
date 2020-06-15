using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugTools : MonoBehaviour
{
    public Camera cameraOverview;
    public Camera cameraPlayer;
    public Image imgCrosshair;
    public TextMeshProUGUI textWeaponName;
    public TextMeshProUGUI textInfo1;
    public TextMeshProUGUI textInfo2;

    // Start is called before the first frame update
    void Start()
    {
        ButtonSwitchCamera(false);
    }

    // Update is called once per frame
    void Update()
    {
        WeaponHolder weaponHolder = GameObject.Find("Player").GetComponent<WeaponHolder>();
        if(weaponHolder != null && weaponHolder.activeWeapon != null)
        {
            textWeaponName.text = weaponHolder.activeWeapon.m_template.GetWeaponName();
            if (weaponHolder.activeWeapon is GunWeapon gun)
            {
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
                textInfo1.gameObject.SetActive(true);
                textInfo2.gameObject.SetActive(true);
                textInfo1.text = "Grenades remaining: " + grenade.m_grenadeCount;
                textInfo2.text = "(Press G to add a grenade)";
            }
            else
            {
                textInfo1.gameObject.SetActive(false);
                textInfo2.gameObject.SetActive(false);
            }
        }

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
        if (cameraPlayer == null || cameraOverview == null)
            return;

        cameraPlayer.enabled = player;
        cameraPlayer.gameObject.GetComponent<AudioListener>().enabled = player;

        cameraOverview.enabled = !player;
        cameraOverview.gameObject.GetComponent<AudioListener>().enabled = !player;

        Cursor.lockState = player ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = !player;
        imgCrosshair.enabled = player;
    }
}
