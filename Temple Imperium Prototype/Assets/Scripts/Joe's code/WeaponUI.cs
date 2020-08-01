using TMPro;
using UnityEngine;

//------------------------------------------------------\\
//  Used to display info in the user interface about    \\
//  the weapon that the player is currently holding     \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class WeaponUI : MonoBehaviour
{
    //Set in inspector:
    public TextMeshProUGUI textWeaponName;  //UI text showing the current weapon being held
    public TextMeshProUGUI textInfo1;       //UI text for displaying debug info
    public TextMeshProUGUI textInfo2;       //Second line of text for displaying debug info

    private WeaponHolder weaponHolder;      //The player's weapon holder component

    // Start is called before the first frame update
    void Start()
    {
        weaponHolder = GameObject.Find("Player").GetComponent<WeaponHolder>();
    }

    void Update()
    {
        if (weaponHolder != null && weaponHolder.activeWeapon != null)
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
            else if (weaponHolder.activeWeapon is GrenadeWeapon grenade)
            {
                //Extra info to display for grenades: number remaining and shortcut info
                textInfo1.gameObject.SetActive(true);
                textInfo2.gameObject.SetActive(false);
                textInfo1.text = "Grenades remaining: " + grenade.m_grenadeCount;
            }
            else
            {
                //Hide extra info when it is not needed
                textInfo1.gameObject.SetActive(false);
                textInfo2.gameObject.SetActive(false);
            }
        }
    }
}
