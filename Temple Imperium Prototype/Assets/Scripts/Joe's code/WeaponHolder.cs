using System;
using System.Collections;
using UnityEngine;

//------------------------------------------------------\\
//  Can be applied to a player or enemy to allow them   \\
//  to hold and fire/attack with weapons                \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class WeaponHolder : MonoBehaviour
{
    //Set in inspector:
    [Header("Generic Weapon Holder Properties")]
    [SerializeField]
    private bool playerControlsWeapons;                 //Can the player control weapon firing/switching? Should be true for the player, false for other entities like enemies
    [SerializeField]
    private WeaponTemplate[] availableWeaponTemplates;  //All weapon types that this weapon holder will contain
    [SerializeField]
    private Transform transformHead;                    //Head of the entity used for positioning weapons
    [SerializeField]
    private GameObject prefabFireLight;                 //Light that is spawned for a short time when a gun is fired

    [Header("Player Specific Properties")]  //These properties only need to be set if the playerControlsWeapons is true
    [SerializeField]
    private Camera playerCamera;            //The first person camera attatched to the player
    [SerializeField]
    private float defaultCameraFOV = 90f;   //The first person camera's default field of view
    [SerializeField]
    private float adsCameraFOV = 60f;       //The first person camera's field of view when aiming down sights

    public starStoneManager generatorStates { get; private set; }//Used to keep track of the active StarStone
    public Weapon activeWeapon { get; private set; }            //The weapon that is currently being held
    private bool emptyHand;                                     //If true, no weapon is displayed
    private Weapon[] availableWeapons;                          //All posible weapons that can be switched between
    private GameObject goWeapon;                                //The held weapon GameObject
    private WeaponAimInfo weaponAimInfo;                        //Contains data regarding where the entity is aiming/what they are aiming at
    private Quaternion targetWeaponRotation;                    //Rotation of the held weapon to lerp towards
    private float targetCameraFOV;                              //First person camera field of view to lerp towards
    private bool aimDownSights;                                 //Is the entity currently aiming down sights

    private void Start()
    {
        //Find generatorStates script and set defaults
        generatorStates = GameObject.FindGameObjectWithTag("GeneratorManager").GetComponent<starStoneManager>();
        targetCameraFOV = defaultCameraFOV;
        SetupAvailableWeapons();
        SwitchActiveWeapon(0, true);
    }

    private void SetupAvailableWeapons()
    {
        //Add all available weapons based on the weapon templates that were set in the inspector.
        //  Each weapon template has a corresponding weapon class, any unrecognised templates wil throw an error
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

    void Update()
    {
        //Aim info to use for this frame
        weaponAimInfo = GetWeaponAimInfo();

        //Call Update on all weapons each frame
        for (int i = 0; i < availableWeapons.Length; i++)
        {
            availableWeapons[i].Update();
        }
        //Call HeldUpdate only on the weapon that is currently being held
        if (activeWeapon != null)
        {
            activeWeapon.HeldUpdate();
        }

        //Player specific updates
        if (playerControlsWeapons)
        {
            CheckForWeaponSwitchInput();
            CheckForAttackInput();
            CheckForWeaponActionInput();
            UpdateWeaponPosition();
            UpdateCamera();
        }

        //Hide the active weapon if the entity should have an empty hand
        if (emptyHand)
        {
            goWeapon.SetActive(false);
        }

        //Aiming down sights is activated by pressing the right mouse button with a gun or prototype weapon
        if ((activeWeapon is GunWeapon || activeWeapon is PrototypeWeapon) && Input.GetButton("Fire2"))
        {
            aimDownSights = true;
        }
        else { aimDownSights = false; }

        //Turn the primary weapon torch on/off if the L key is pressed
        if (activeWeapon is GunWeapon activeGun && Input.GetKeyDown(KeyCode.L))
        {
            bool torchOn = activeGun.ToggleTorchOn();
            EnableWeaponTorchGameObject(torchOn);
        }
    }

    private void UpdateCamera()
    {
        //Change camera FOV depending on if the player is aiming down sights
        if (aimDownSights)
        {
            targetCameraFOV = adsCameraFOV;
        }
        else
        {
            targetCameraFOV = defaultCameraFOV;
        }
        //Lerp to the target FOV for the current one to create smooth motion
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetCameraFOV, Time.deltaTime * 20f);
    }

    public void SetEmptyHand(bool empty)
    {
        //Toggles whether the entity has an empty hand
        emptyHand = empty;
        if (!empty)
        {
            //Holding weapon - switch back to the active weapon
            if (activeWeapon != null)
            {
                SwitchActiveWeapon(Array.IndexOf(availableWeapons, activeWeapon), true);
            }
        }
        else
        {
            //Empty hand - ensure weapon usage is disabled
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
        //Move through all available weapons when scrolling,
        //  scroll direction determines if switching forwards/backwards
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            CycleThroughAvailableWeapons(true);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            CycleThroughAvailableWeapons(false);
        }

        //Switch to the weapon corresponding to any pressed number key
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
        //Find the index of the current weapon, or default to 0 if not holding a weapon
        int currentWeaponIndex = 0;
        if (activeWeapon != null)
        {
            currentWeaponIndex = Array.IndexOf(availableWeapons, activeWeapon);
        }

        int nextWeaponIndex;
        //Set the next weapon index to either the next or previous weapon
        if (forwards)
        {
            //Resets back to 0 if at the last weapon in the list
            nextWeaponIndex = 0;
            if (currentWeaponIndex < (availableWeapons.Length - 1))
            {
                nextWeaponIndex = currentWeaponIndex + 1;
            }
        }
        else
        {
            //Resets to the final weapon if at the first weapon in the list
            nextWeaponIndex = (availableWeapons.Length - 1);
            if (currentWeaponIndex > 0)
            {
                nextWeaponIndex = currentWeaponIndex - 1;
            }
        }

        //Swich based on the index thet was determined
        SwitchActiveWeapon(nextWeaponIndex);
    }
    public void SwitchActiveWeapon(int weaponIndex, bool forceSwitch = false)
    {
        //Ensure the given index is a valid index for the availableWeapons array
        Weapon weapon;
        if (weaponIndex >= 0 && weaponIndex < availableWeapons.Length)
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
            //Remove the GameObject for the previously held weapon
            if (goWeapon != null)
            {
                Destroy(goWeapon);
            }
            if (weapon != null)
            {
                //Trigger any SwitchingToOtherWeapon events on the active weapon
                if (activeWeapon != null)
                {
                    activeWeapon.SwitchingToOtherWeapon();
                }

                //Set the new active weapon and create its GameObject as determined by the template
                activeWeapon = weapon;
                goWeapon = Instantiate(activeWeapon.m_template.GetGameObject(), transformHead);
                goWeapon.transform.localPosition += activeWeapon.m_template.GetVisualOffset();
                SetHeldWeaponHidden(weapon.m_hideHeldWeapon);

                //Trigger SwitchingToThisWeapon events on the new weapon
                weapon.SwitchingToThisWeapon();

                //Toggle the torch if switching to a gun that has one
                if (weapon is GunWeapon gun)
                {
                    EnableWeaponTorchGameObject(gun.m_torchOn);
                }
            }
        }
    }
    public void SetHeldWeaponHidden(bool hide)
    {
        goWeapon.SetActive(!hide);
    }
    private void EnableWeaponTorchGameObject(bool torchOn)
    {
        //Enables/disabled the torch GameObject
        Transform transformTorch = goWeapon.transform.Find("Torch");
        if (transformTorch != null)
        {
            transformTorch.gameObject.SetActive(torchOn);
        }
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
        //Alternate attack (used for weapons with multiple uses, e.g. guns with melee ability)
        else if (Input.GetButton("AttackAlternate") && !aimDownSights)
        {
            TryUsingWeaponAlternate();
        }
        //For weapons with a continuous attack, end weapon usage on mouse up
        if (Input.GetButtonUp("Fire1"))
        {
            TryEndingWeaponUsage();
        }
    }
    private void CheckForWeaponActionInput()
    {
        //If holding a gun, reload when the R key is pressed
        //  (Guns auto reload when out of ammo, this gives the option for a manual reload at any time)
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(activeWeapon != null && activeWeapon is GunWeapon activeGun)
            {
                activeGun.StartReload();
            }
        }
    }
    public void TryUsingWeapon(bool buttonDown)
    {
        //Attacks if a weapon is being held at is ready to be used
        if (!emptyHand && activeWeapon.ReadyToAttack())
        {
            activeWeapon.Attack(weaponAimInfo, goWeapon, prefabFireLight, transformHead, buttonDown);
        }
    }
    public void TryUsingWeaponAlternate()
    {
        //Secondary attack if a weapon is being held at its alternate attack is ready to be used
        if (!emptyHand && activeWeapon.ReadyToAttackAlternate())
        {
            activeWeapon.AlternateAttack(weaponAimInfo, goWeapon, transformHead);
        }
    }
    public void TryEndingWeaponUsage()
    {
        //Stops the attack for weapons that stay active once triggered
        if (activeWeapon is PrototypeWeapon activeProto)
        {
            activeProto.StopAttack();
        }
    }
    #endregion

    public void PickupAmmo(int amount, GunWeaponTemplate gunType)
    {
        //Finds a weapon with the specified template and adds a set amount of ammo
        for (int i = 0; i < availableWeapons.Length; i++)
        {
            if (availableWeapons[i].m_template == gunType)
            {
                ((GunWeapon)availableWeapons[i]).AddAmmo(amount);
                return;
            }
        }
    }

    private WeaponAimInfo GetWeaponAimInfo()
    {
        //Find the maximum distance the active weapon can fire/attack based on its range,
        //  or use a default of 10 for weapons that do not specify a range
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

        //Check if the weapon is aiming at an object using a raycast, ignoring the player
        //  if they are holding the weapon to prevent them from hitting themselves
        LayerMask layerMask = 0;
        if (playerControlsWeapons) { layerMask = (~LayerMask.GetMask("Player")); }
        if (Physics.Raycast(transformHead.position, transformHead.forward, out RaycastHit hit, maxDistance, layerMask))
        {
            //Hit: Return WeaponAimInfo with the raycast result
            return new WeaponAimInfo(hit.point, true, hit, maxDistance);
        }
        else
        {
            //No hit: Return WeaponAimInfo using the maxDistance for the weapon
            return new WeaponAimInfo((transformHead.position + (transformHead.forward * maxDistance)), false, new RaycastHit(), maxDistance);
        }
    }

    //Destroys a weapon GameObject after a set amount of time,
    //  useful for GO's used for effects that should last for a set amount of time and then disappear
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


    public PrototypeWeapon GetPrototypeWeapon()
    {
        //Returns a prototype weapon if one can be found in the entities available weapons,
        //  or throws an error is none are available
        for (int i = 0; i < availableWeapons.Length; i++)
        {
            if (availableWeapons[i] is PrototypeWeapon protoWeapon)
                return protoWeapon;
        }
        Debug.LogError("Trying to get prototype weapon on entity that is not holding one (" + gameObject.name + ")");
        return null;
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
    //Properties
    public Vector3 m_aimPoint { get; private set; }     //The position the weapon should aim towards
    public bool m_raycastHit { get; private set; }      //Whether anything was hit by the last raycast
    public RaycastHit m_hitInfo { get; private set; }   //Info from the last raycast if anything was hit
    public float m_maxDistance { get; private set; }    //The maximum distance to aim for if nothing was hit

    //Constructor
    public WeaponAimInfo(Vector3 aimPoint, bool raycastHit, RaycastHit hitInfo, float maxDistance)
    {
        m_aimPoint = aimPoint;
        m_raycastHit = raycastHit;
        m_hitInfo = hitInfo;
        m_maxDistance = maxDistance;
    }
}