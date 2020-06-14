using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    //Set in inspector:
    public bool playerControlsWeapons;  //Can the player control weapon firing/switching? Should be true for the player, false for enemies
    public WeaponTemplate[] availableWeaponTemplates;
    public Weapon activeWeapon;
    public Transform transformHead;
    public GameObject prefabFireLight;

    public int ammo { get; set; } = 100;
    private Weapon[] availableWeapons;
    private GameObject goWeapon;

    private void Start()
    {
        SetupAvailableWeapons();

        SwitchActiveWeapon(0, true);
    }

    private void SetupAvailableWeapons()
    {
        availableWeapons = new Weapon[availableWeaponTemplates.Length];
        for (int i = 0; i < availableWeaponTemplates.Length; i++)
        {
            if (availableWeaponTemplates[i] is GunWeaponTemplate gunTemplate)
            {
                availableWeapons[i] = new GunWeapon(this, gunTemplate);
            }
            else if (availableWeaponTemplates[i] is MeleeWeaponTemplate meleeTemplate)
            {
                availableWeapons[i] = new MeleeWeapon(this, meleeTemplate);
            }
            else if (availableWeaponTemplates[i] is GrenadeWeaponTemplate grenadeTemplate)
            {
                availableWeapons[i] = new GrenadeWeapon(this, grenadeTemplate);
            }
            else if (availableWeaponTemplates[i] is PrototypeWeaponTemplate prototypeTemplate)
            {
                availableWeapons[i] = new PrototypeWeapon(this, prototypeTemplate);
            }
            else
            {
                Debug.LogError("Unrecognised weapon type at setup: " + availableWeaponTemplates[i].GetWeaponName());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Call Update on all weapons
        for (int i = 0; i < availableWeapons.Length; i++)
        {
            availableWeapons[i].Update();
        }

        //Call HeldUpdate on the weapon that is currently being held
        if (activeWeapon != null)
        {
            activeWeapon.HeldUpdate();
        }

        if (playerControlsWeapons)
        {
            //DELETE ME :: DEBUG AMMO INCREASE
            if (Input.GetKeyDown(KeyCode.P))
            {
                ammo += 10;
            }
            CheckForWeaponSwitchInput();
            CheckForAttackInput();
            UpdateWeaponPosition();
        }
    }

    private void UpdateWeaponPosition()
    {
        if ((activeWeapon is GunWeapon activeGun) && goWeapon != null)
        {
            Vector3 targetGunPos = activeGun.m_gunTemplate.GetVisualOffset();
            Vector3 targetGunRotation = new Vector3(0f, activeGun.m_template.GetYRotation(), 0f);
            if (Input.GetButton("Fire2"))
            {
                targetGunPos = activeGun.m_gunTemplate.GetAimDownSightOffset();
                targetGunRotation = Vector3.zero;
            }
            goWeapon.transform.localPosition = Vector3.Lerp(goWeapon.transform.localPosition, targetGunPos, Time.deltaTime * 20f);
            goWeapon.transform.localRotation = Quaternion.Lerp(goWeapon.transform.localRotation, Quaternion.Euler(targetGunRotation), Time.deltaTime * 20f);
        }
    }

    #region Weapon Switching
    private void CheckForWeaponSwitchInput()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            CycleThroughAvailableWeapons(true);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            CycleThroughAvailableWeapons(false);
        }

        for (int i = 0; i < availableWeapons.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                SwitchActiveWeapon(i);
            }
        }
    }
    private void CycleThroughAvailableWeapons(bool forwards)
    {
        int currentWeaponIndex = 0;
        if(activeWeapon != null)
        {
            currentWeaponIndex = Array.IndexOf(availableWeapons, activeWeapon);
        }

        int nextWeaponIndex;
        if (forwards)
        {
            nextWeaponIndex = 0;
            if (currentWeaponIndex < (availableWeapons.Length - 1))
            {
                nextWeaponIndex = currentWeaponIndex + 1;
            }
        }
        else
        {
            nextWeaponIndex = (availableWeapons.Length - 1);
            if(currentWeaponIndex > 0)
            {
                nextWeaponIndex = currentWeaponIndex - 1;
            }
        }

        SwitchActiveWeapon(nextWeaponIndex);
    }
    public void SwitchActiveWeapon(int weaponIndex, bool forceSwitch = false)
    {
        Weapon weapon;
        if(weaponIndex >= 0 && weaponIndex < availableWeapons.Length)
        {
            weapon = availableWeapons[weaponIndex];
        }
        else
        {
            Debug.LogError("Invalid weapon switch index for " + gameObject.name + ": " + weaponIndex);
            return;
        }

        //Only switch weapon if the new specified weapon is not already in use
        //  or a switch is being forced (used for setting up a weapon in the inspector before playing)
        if ((activeWeapon != weapon) || forceSwitch)
        {
            if (goWeapon != null)
            {
                Destroy(goWeapon);
            }
            if (weapon != null)
            {
                activeWeapon = weapon;
                goWeapon = Instantiate(activeWeapon.m_template.GetGameObject(), transformHead);
                goWeapon.transform.localPosition += activeWeapon.m_template.GetVisualOffset();
                goWeapon.transform.localRotation = Quaternion.Euler(0f, activeWeapon.m_template.GetYRotation(), 0f);
                SetHeldWeaponHidden(weapon.m_hideHeldWeapon);
            }
        }
    }
    public void SetHeldWeaponHidden(bool hide)
    {
        goWeapon.SetActive(!hide);
    }
    #endregion

    #region Weapon Usage
    private void CheckForAttackInput()
    {
        //Try firing the held weapon if the fire button is being pressed
        if (Input.GetButton("Fire1"))
        {
            bool buttonDown = Input.GetButtonDown("Fire1");
            TryUsingWeapon(buttonDown);
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            TryEndingWeaponUsage();
        }
    }
    public void TryUsingWeapon(bool buttonDown)
    {
        if (activeWeapon.ReadyToFire())
        {
            activeWeapon.Attack(goWeapon, prefabFireLight, transformHead, buttonDown);
        }
    }
    public void TryEndingWeaponUsage()
    {
        if(activeWeapon is PrototypeWeapon activeProto)
        {
            activeProto.DisableBeam(transformHead);
        }
    }
    #endregion

    public void PickupAmmo(int amount)
    {
        ammo += amount;
    }

    private void OnDrawGizmos()
    {
        //Debug visualisation for weapon aiming

        if (playerControlsWeapons)
        {
            float range = 1f;
            if (activeWeapon is GunWeapon activeGun)
            {
                range = activeGun.m_gunTemplate.GetRange();
            }
            else if (activeWeapon is MeleeWeapon activeMelee)
            {
                range = activeMelee.m_meleeTemplate.GetRange();
            }

            Vector3 endPoint;
            if (Physics.Raycast(transformHead.position, transformHead.forward, out RaycastHit hitInfo, range))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transformHead.position, transformHead.forward * hitInfo.distance);
                Gizmos.DrawSphere(hitInfo.point, 0.1f);
                endPoint = hitInfo.point;
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transformHead.position, transformHead.forward * range);
                endPoint = transformHead.position + (transformHead.forward * range);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transformHead.position, new Vector3(endPoint.x, transformHead.position.y, endPoint.z));
            Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
            Gizmos.DrawLine(endPoint, new Vector3(endPoint.x, transformHead.position.y, endPoint.z));
        }
    }
}
