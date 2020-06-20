using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TempStarStoneState
{
    None,
    Power_Purple,
    Heat_Orange,
    Ice_Blue,
    Heal_Pink
}

public class WeaponHolder : MonoBehaviour
{
    //Set in inspector:
    [Header("Generic Weapon Holder Properties")]

    [SerializeField]
    private bool playerControlsWeapons;  //Can the player control weapon firing/switching? Should be true for the player, false for enemies
    [SerializeField]
    private WeaponTemplate[] availableWeaponTemplates;
    [SerializeField]
    private Transform transformHead;
    [SerializeField]
    private GameObject prefabFireLight;

    [Header("Player Specific Properties")]

    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private float defaultCameraFOV = 90f;
    [SerializeField]
    private float adsCameraFOV = 60f;

    //public int ammo { get; set; } = 100;
    public generatorStates generatorStates { get; private set; }
    public Weapon activeWeapon { get; private set; }
    private bool emptyHand;
    private Weapon[] availableWeapons;
    private GameObject goWeapon;
    private WeaponAimInfo weaponAimInfo;
    private Quaternion targetWeaponRotation;
    private float targetCameraFOV;

    private void Start()
    {
        generatorStates = GameObject.FindGameObjectWithTag("GeneratorManager").GetComponent<generatorStates>();

        targetCameraFOV = defaultCameraFOV;

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
        weaponAimInfo = GetWeaponAimInfo();

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
            CheckForWeaponSwitchInput();
            CheckForAttackInput();
            UpdateWeaponPosition();
            UpdateCamera();
        }

        if (emptyHand)
        {
            goWeapon.SetActive(false);
        }
    }

    private void UpdateCamera()
    {
        //Aiming down sights
        if( (activeWeapon is GunWeapon || activeWeapon is PrototypeWeapon) && Input.GetButton("Fire2"))
        {
            targetCameraFOV = adsCameraFOV;
        }
        else
        {
            targetCameraFOV = defaultCameraFOV;
        }
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetCameraFOV, Time.deltaTime * 20f);
    }

    public void SetEmptyHand(bool empty)
    {
        emptyHand = empty;

        if(!empty)
        {
            if(activeWeapon != null)
            {
                SwitchActiveWeapon(Array.IndexOf(availableWeapons, activeWeapon), true);
            }
        }
        else
        {
            TryEndingWeaponUsage();
        }
    }

    private void UpdateWeaponPosition()
    {
        if (goWeapon != null)
        {
            bool rotationChanged = false;
            Vector3 targetWeaponPos;

            if ((activeWeapon is GunWeapon activeGun) && Input.GetButton("Fire2"))
            {
                targetWeaponPos = activeGun.m_gunTemplate.GetAimDownSightOffset();
                targetWeaponRotation = goWeapon.transform.parent.rotation;
                rotationChanged = true;
            }
            else if ((activeWeapon is PrototypeWeapon activeProto) && Input.GetButton("Fire2"))
            {
                targetWeaponPos = activeProto.m_prototypeTemplate.GetAimDownSightOffset();
                targetWeaponRotation = goWeapon.transform.parent.rotation;
                rotationChanged = true;
            }
            else
            {
                targetWeaponPos = activeWeapon.m_template.GetVisualOffset();

                //Rotate weapon to aim weapon at target
                //  - unless distance to target is very low, to avoid weapons trying to aim backwards
                if (weaponAimInfo.m_hitInfo.distance > 1.2f)
                {
                    Vector3 weaponLookDirection = weaponAimInfo.m_aimPoint - goWeapon.transform.Find("AimPoint").position;
                    targetWeaponRotation = Quaternion.LookRotation(weaponLookDirection.normalized, goWeapon.transform.parent.up);
                    rotationChanged = true;
                }
            }

            goWeapon.transform.localPosition = Vector3.Lerp(goWeapon.transform.localPosition, targetWeaponPos, Time.deltaTime * 20f);
            if (rotationChanged)
            {
                goWeapon.transform.rotation = Quaternion.Lerp(goWeapon.transform.rotation, targetWeaponRotation, Time.deltaTime * 20f);
            }
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
                if(activeWeapon != null)
                {
                    activeWeapon.SwitchingToOtherWeapon();
                }
                activeWeapon = weapon;
                goWeapon = Instantiate(activeWeapon.m_template.GetGameObject(), transformHead);
                goWeapon.transform.localPosition += activeWeapon.m_template.GetVisualOffset();
                SetHeldWeaponHidden(weapon.m_hideHeldWeapon);
                weapon.SwitchingToThisWeapon();
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
        if (Input.GetButtonUp("Fire1"))
        {
            TryEndingWeaponUsage();
        }
    }
    public void TryUsingWeapon(bool buttonDown)
    {
        if (!emptyHand && activeWeapon.ReadyToFire())
        {
            activeWeapon.Attack(weaponAimInfo, goWeapon, prefabFireLight, transformHead, buttonDown);
        }
    }
    public void TryEndingWeaponUsage()
    {
        if(activeWeapon is PrototypeWeapon activeProto)
        {
            activeProto.StopAttack();
        }
    }
    #endregion

    public void PickupAmmo(int amount, GunWeaponTemplate gunType)
    {
        for (int i = 0; i < availableWeapons.Length; i++)
        {
            if(availableWeapons[i].m_template == gunType)
            {
                ((GunWeapon)availableWeapons[i]).AddAmmo(amount);
                return;
            }
        }
    }

    private WeaponAimInfo GetWeaponAimInfo()
    {
        float maxDistance = 10f;
        if (activeWeapon is GunWeapon activeGun)
        {
            maxDistance = activeGun.m_gunTemplate.GetRange();
        }
        else if (activeWeapon is MeleeWeapon activeMelee)
        {
            maxDistance = activeMelee.m_meleeTemplate.GetRange();
        }
        else if (activeWeapon is PrototypeWeapon activeProto)
        {
            maxDistance = activeProto.m_prototypeTemplate.GetRange();
        }

        if (Physics.Raycast(transformHead.position, transformHead.forward, out RaycastHit hit, maxDistance, ~LayerMask.GetMask("Player")))
        {
            return new WeaponAimInfo(hit.point, true, hit, maxDistance);
        }
        else
        {
            return new WeaponAimInfo((transformHead.position + (transformHead.forward * maxDistance)), false, new RaycastHit(), maxDistance);
        }
    }

    public void DestroyWeaponGameObjectAfterTime(GameObject goWeapon, float time)
    {
        StartCoroutine(DestroyWeaponGameObjectAfterTimeCoroutine(goWeapon, time));
    }

    private IEnumerator DestroyWeaponGameObjectAfterTimeCoroutine(GameObject goWeapon, float time)
    {
        yield return new WaitForSeconds(time);
        if (goWeapon != null)
        {
            Destroy(goWeapon);
        }
    }

    private void OnDrawGizmos()
    {
        //Debug visualisation for weapon aiming

        if (playerControlsWeapons)
        {
            Vector3 focusPoint = weaponAimInfo.m_aimPoint;

            if (weaponAimInfo.m_raycastHit)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transformHead.position, transformHead.forward * weaponAimInfo.m_hitInfo.distance);
                Gizmos.DrawSphere(weaponAimInfo.m_hitInfo.point, 0.1f);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transformHead.position, transformHead.forward * weaponAimInfo.m_maxDistance);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transformHead.position, new Vector3(focusPoint.x, transformHead.position.y, focusPoint.z));
            Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
            Gizmos.DrawLine(focusPoint, new Vector3(focusPoint.x, transformHead.position.y, focusPoint.z));
        }
    }
}

public struct WeaponAimInfo
{
    public Vector3 m_aimPoint { get; private set; }
    public bool m_raycastHit { get; private set; }
    public RaycastHit m_hitInfo { get; private set; }
    public float m_maxDistance { get; private set; }
    public WeaponAimInfo(Vector3 aimPoint, bool raycastHit, RaycastHit hitInfo, float maxDistance)
    {
        m_aimPoint = aimPoint;
        m_raycastHit = raycastHit;
        m_hitInfo = hitInfo;
        m_maxDistance = maxDistance;
    }
}