using UnityEngine;

//------------------------------------------------------\\
//  A proto weapon that can be used by an entity with   \\
//  a WeaponHolder that holds a PrototypeWeaponTemplate.\\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class PrototypeWeapon : Weapon
{
    //Properties
    public PrototypeWeaponTemplate m_prototypeTemplate { get; private set; }    //The template that defines the weapon's base properties
    public float m_damagePower { get; private set; }    //The amount of power that increases as the weapon is fired and determines the amount of damage dealt
    public bool m_poweringUp { get; private set; }      //True while the weapon is being fired
    public float m_charge { get; private set; }         //The amount of charge (from StarStones) this weapon has

    private bool m_charging;        //True while charging at the generator
    private float m_damageTimer;    //Amount of time (seconds) until damage is dealt
    private GameObject m_goWeapon;  //GameObject for the held weapon
    private GameObject m_goBeam;    //GameObject for the beam that is fired from the weapon

    public void SetCharging(bool charging) { m_charging = charging; }

    //Constructor
    public PrototypeWeapon(WeaponHolder weaponHolder, PrototypeWeaponTemplate template) : base(weaponHolder, template)
    {
        m_prototypeTemplate = template;
        m_charge = m_prototypeTemplate.GetMaxCharge();
    }

    //Called when switching from another weapon
    public override void SwitchingToThisWeapon()
    {
        m_poweringUp = false;
    }

    //Called when switching from this weapon to another, ensures firing and beam sound are stopped
    public override void SwitchingToOtherWeapon()
    {
        m_poweringUp = false;
        AudioManager.instance.StopLoopingSoundEffect("protoBeam");
    }

    public override bool ReadyToAttack()
    {
        //Can attack if this weapon has some charge
        if(base.ReadyToAttack() && m_charge > 0f)
        {
            return true;
        }
        return false;
    }

    public override void HeldUpdate()
    {
        base.HeldUpdate();

        if (m_poweringUp)
        {
            //While firing, increase damage power until it reaches its max value
            //  so more damage is dealt as the weapon is fired
            m_damagePower += Time.deltaTime;
            if (m_damagePower >= 1f)
            {
                m_damagePower = 1f;
            }
            //Also reduce the damage timer to ensure damage is dealt after a set time
            m_damageTimer -= Time.deltaTime;
        }

        if (m_goBeam != null)
        {
            //Incerase beam width as damage power increases, unless the purple StarStone is active,
            //  in which case the beam should stay at a set width
            float beamWidth;
            if (m_weaponHolder.generatorStates.returnActive() == starStoneManager.starStones.Purple)
            {
                beamWidth = 0.4f;
            }
            else
            {
                beamWidth = (m_damagePower * 0.4f);
            }
            m_goBeam.transform.Find("Beam").localScale = new Vector3(beamWidth, (m_prototypeTemplate.GetRange() / 2f) - 0.5f, beamWidth);
        }
    }

    public override void Update()
    {
        base.Update();

        //While charging, increase the charge based on the value set in the weapon's template
        //  (using deltaTime to keep charge speed framerate independent)
        if (m_charging)
        {
            float maxCharge = m_prototypeTemplate.GetMaxCharge();
            if (m_charge < maxCharge)
            {
                m_charge += (Time.deltaTime * m_prototypeTemplate.GetChargeSpeed());
            }
            else
            {
                m_charge = maxCharge;
            }
        }
    }

    public override void Attack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {
        //Generic attack method that is called for all StarStone states

        //Reset attack interval to prevent continuous firing
        m_attackIntervalTimer = m_template.GetAttackInterval();

        m_goWeapon = weaponGameObject;

        //Decrease charge while firing so the weapon cannot be fired forever
        if(m_charge > 0f)
        {
            m_charge -= (Time.deltaTime * m_prototypeTemplate.GetChargeDrainSpeed());
        }

        //Stop the attack if StarStone charge has run out
        if(m_charge <= 0f)
        {
            StopAttack();
            m_charge = 0f;
            return;
        }

        //Start powering up if not already
        starStoneManager.starStones generatorState = m_weaponHolder.generatorStates.returnActive();
        if (!m_poweringUp)
        {
            StartPoweringUp();
            //For the purple StarStone, a beam is created and destroyed after a short amount of time
            //  rather than being visible for the duration of the attack
            if (generatorState != starStoneManager.starStones.Purple)
                CreateBeamGameObject(weaponGameObject);
        }

        //Call specific attack methods based on the active StarStone
        switch (generatorState)
        {
            case starStoneManager.starStones.None:
                DefaultAttack(weaponAimInfo);
                break;
            case starStoneManager.starStones.Orange:
                HeatAttack(weaponAimInfo);
                break;
            case starStoneManager.starStones.Purple:
                PowerAttack(weaponAimInfo, weaponGameObject);
                break;
            case starStoneManager.starStones.Blue:
                IceAttack(weaponAimInfo);
                break;
            case starStoneManager.starStones.Pink:
                HealAttack(weaponAimInfo);
                break;
        }

        //Play the shooting animation
        weaponGameObject.transform.Find("Weapon").GetComponent<Animator>().SetBool("Shooting", true);
    }

    //No alternate attack for prototype weapons
    public override void AlternateAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, Transform transformHead) {}

    private void DefaultAttack(WeaponAimInfo weaponAimInfo)
    {
        if (m_damageTimer <= 0)
        {
            m_damageTimer = m_prototypeTemplate.GetDamageInterval();

            if (weaponAimInfo.m_raycastHit)
            {
                //Beam hit an object
                Debug.Log("Proto weapon firing, hitting " + weaponAimInfo.m_hitInfo.transform.name);

                GameObject goHit = weaponAimInfo.m_hitInfo.collider.gameObject;
                if (goHit.CompareTag("Enemy"))
                {
                    //Hit an enemy, apply scaled damage based on damage power
                    float damagePerc = m_damagePower / 1f;
                    int scaledDamage = Mathf.RoundToInt( RemapNumber(damagePerc, 0f, 1f, m_template.GetMinAttackDamage(), m_template.GetMaxAttackDamage()) );
                    weaponAimInfo.m_hitInfo.transform.GetComponent<Enemy>().Damage(scaledDamage);
                    UIManager.instance.ShowEnemyHitPopup(scaledDamage, weaponAimInfo.m_hitInfo.point);
                }
                //Explode any hit objects with the ExplodeOnImpact component
                else if (goHit.CompareTag("ExplodeOnImpact"))
                {
                    goHit.GetComponent<ExplodeOnImpact>().Explode();
                }
            }
            else
            {
                Debug.Log("Proto weapon firing, hitting nothing");
            }
        }
    }

    private void HeatAttack(WeaponAimInfo weaponAimInfo)
    {
        if (m_damageTimer <= 0)
        {
            m_damageTimer = m_prototypeTemplate.GetDamageInterval();

            if (weaponAimInfo.m_raycastHit)
            {
                //Beam hit an object
                Debug.Log("[HEAT] Proto weapon firing, hitting " + weaponAimInfo.m_hitInfo.transform.name);

                GameObject goHit = weaponAimInfo.m_hitInfo.collider.gameObject;
                if (goHit.CompareTag("Enemy"))
                {
                    //Hit an enemy, apply scaled damage based on damage power
                    float damagePerc = m_damagePower / 1f;
                    int scaledDamage = Mathf.RoundToInt(RemapNumber(damagePerc, 0f, 1f, m_template.GetMinAttackDamage(), m_template.GetMaxAttackDamage()));
                    Enemy hitEnemy = weaponAimInfo.m_hitInfo.transform.GetComponent<Enemy>();
                    hitEnemy.Damage(scaledDamage);
                    //Also set the enemy on fire
                    hitEnemy.setOnFire(m_prototypeTemplate.GetFireEffectTime(), m_prototypeTemplate.GetFireDamage(), m_prototypeTemplate.GetTimeBetweenFireDamage());
                    UIManager.instance.ShowEnemyHitPopup(scaledDamage, weaponAimInfo.m_hitInfo.point);
                }
                //Explode any hit objects with the ExplodeOnImpact component
                else if (goHit.CompareTag("ExplodeOnImpact"))
                {
                    goHit.GetComponent<ExplodeOnImpact>().Explode();
                }
            }
            else
            {
                Debug.Log("[HEAT] Proto weapon firing, hitting nothing");
            }
        }
    }

    private void PowerAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject)
    {
        //Fully charged and ready to shoot
        if(m_damagePower == 1f)
        {
            CreateBeamGameObject(weaponGameObject);

            if (weaponAimInfo.m_raycastHit)
            {
                //Beam hit an object
                Debug.Log("[POWER] Proto weapon firing, hitting " + weaponAimInfo.m_hitInfo.transform.name);

                GameObject goHit = weaponAimInfo.m_hitInfo.collider.gameObject;
                if (goHit.CompareTag("Enemy"))
                {
                    //Hit an enemy, apply power damage as set in the weapon's template
                    int damageAmount = Random.Range(m_prototypeTemplate.GetMinPowerDamage(), m_prototypeTemplate.GetMaxPowerDamage() + 1);
                    Enemy hitEnemy = weaponAimInfo.m_hitInfo.transform.GetComponent<Enemy>();
                    hitEnemy.Damage(damageAmount);
                    UIManager.instance.ShowEnemyHitPopup(damageAmount, weaponAimInfo.m_hitInfo.point);
                }
                //Explode any hit objects with the ExplodeOnImpact component
                else if (goHit.CompareTag("ExplodeOnImpact"))
                {
                    goHit.GetComponent<ExplodeOnImpact>().Explode();
                }
            }
            else
            {
                Debug.Log("[POWER] Proto weapon firing, hitting nothing");
            }

            //Power attack is a one-shot attack, so stop powering up and reset the attack interval
            AudioManager.instance.PlaySoundEffect2D(m_prototypeTemplate.m_powerSound, m_prototypeTemplate.m_powerSoundVolume);
            m_poweringUp = false;
            m_attackIntervalTimer = m_template.GetAttackInterval();
            AudioManager.instance.StopLoopingSoundEffect("protoBeam");
            //Also destroy the beam after a short amount of time so it appears as a flash of light
            m_weaponHolder.DestroyWeaponGameObjectAfterTime(m_goBeam, 0.2f);
        }
    }

    private void IceAttack(WeaponAimInfo weaponAimInfo)
    {
        if (m_damageTimer <= 0)
        {
            m_damageTimer = m_prototypeTemplate.GetDamageInterval();

            if (weaponAimInfo.m_raycastHit)
            {
                //Beam hit an object
                Debug.Log("[ICE] Proto weapon firing, hitting " + weaponAimInfo.m_hitInfo.transform.name);

                GameObject goHit = weaponAimInfo.m_hitInfo.collider.gameObject;
                if (goHit.CompareTag("Enemy"))
                {
                    //Hit an enemy, apply scaled damage based on damage power
                    float damagePerc = m_damagePower / 1f;
                    int scaledDamage = Mathf.RoundToInt(RemapNumber(damagePerc, 0f, 1f, m_template.GetMinAttackDamage(), m_template.GetMaxAttackDamage()));
                    Enemy hitEnemy = weaponAimInfo.m_hitInfo.transform.GetComponent<Enemy>();
                    hitEnemy.Damage(scaledDamage);
                    //Also slow the enemy down for a set amount of time specified in the weapon template
                    hitEnemy.SlowEnemyForTime(m_prototypeTemplate.GetSpeedMultiplier(), m_prototypeTemplate.GetSlowdownTime());
                    UIManager.instance.ShowEnemyHitPopup(scaledDamage, weaponAimInfo.m_hitInfo.point);
                }
                //Explode any hit objects with the ExplodeOnImpact component
                else if (goHit.CompareTag("ExplodeOnImpact"))
                {
                    goHit.GetComponent<ExplodeOnImpact>().Explode();
                }
            }
            else
            {
                Debug.Log("[ICE] Proto weapon firing, hitting nothing");
            }
        }
    }

    private void HealAttack(WeaponAimInfo weaponAimInfo)
    {
        if (m_damageTimer <= 0)
        {
            m_damageTimer = m_prototypeTemplate.GetDamageInterval();

            if (weaponAimInfo.m_raycastHit)
            {
                //Beam hit an object
                Debug.Log("[HEAL] Proto weapon firing, hitting " + weaponAimInfo.m_hitInfo.transform.name);

                GameObject goHit = weaponAimInfo.m_hitInfo.collider.gameObject;
                if (goHit.CompareTag("Enemy"))
                {
                    //Hit an enemy, apply scaled damage based on damage power
                    float damagePerc = m_damagePower / 1f;
                    int scaledDamage = Mathf.RoundToInt(RemapNumber(damagePerc, 0f, 1f, m_template.GetMinAttackDamage(), m_template.GetMaxAttackDamage()));
                    int scaledHealthRestore = Mathf.RoundToInt(RemapNumber(damagePerc, 0f, 1f, 0f, m_prototypeTemplate.GetHealthRestoreAmount()));

                    //If a player is holding this weapon, restore some health
                    if (m_weaponHolder.gameObject.CompareTag("Player"))
                    {
                        m_weaponHolder.gameObject.GetComponent<playerHealth>().RestoreHealth(scaledHealthRestore);
                    }

                    Enemy hitEnemy = weaponAimInfo.m_hitInfo.transform.GetComponent<Enemy>();
                    hitEnemy.Damage(scaledDamage);
                    UIManager.instance.ShowEnemyHitPopup(scaledDamage, weaponAimInfo.m_hitInfo.point);
                }
                //Explode any hit objects with the ExplodeOnImpact component
                else if (goHit.CompareTag("ExplodeOnImpact"))
                {
                    goHit.GetComponent<ExplodeOnImpact>().Explode();
                }
            }
            else
            {
                Debug.Log("[HEAL] Proto weapon firing, hitting nothing");
            }
        }
    }

    public void StopAttack()
    {
        //Disable attack sounds/animations and stop powering up
        m_poweringUp = false;
        m_damagePower = 0f;
        if (m_goBeam != null)
        {
            AudioManager.instance.PlaySoundEffect2D(m_prototypeTemplate.m_disableSound, m_prototypeTemplate.m_disableSoundVolume);
            Object.Destroy(m_goBeam);
        }
        if (m_goWeapon != null)
        {
            m_goWeapon.transform.Find("Weapon").GetComponent<Animator>().SetBool("Shooting", false);
        }
        AudioManager.instance.StopLoopingSoundEffect("protoBeam");
    }

    private void StartPoweringUp()
    {
        //Reset damage power/timer values so that don't carry over from a previous attack
        m_poweringUp = true;
        m_damagePower = 0f;
        m_damageTimer = 0f;

        //Play attack sound and start a looping sonud that plays for the duration of an attack
        AudioManager.instance.PlaySoundEffect2D(m_template.m_attackSound, m_template.m_attackSoundVolume, 0.95f, 1.05f);
        AudioManager.instance.PlayLoopingSoundEffect(m_prototypeTemplate.m_firingSound, false, Vector3.zero, "protoBeam", m_prototypeTemplate.m_firingSoundVolume);
    }

    private void CreateBeamGameObject(GameObject weaponGameObject)
    {

        if (m_goBeam != null)
        {
            Object.Destroy(m_goBeam);
        }

        //Create the beam and position it based on range
        m_goBeam = Object.Instantiate(m_prototypeTemplate.GetBeamGameObject(), weaponGameObject.transform.Find("AimPoint"));

        starStoneManager.starStones starStoneState = m_weaponHolder.generatorStates.returnActive();

        //Set beam and particle materials based on active StarStone
        ParticleSystem.MainModule beamParticles = m_goBeam.transform.Find("Beam Particles").GetComponent<ParticleSystem>().main;
        MeshRenderer beamMeshRen = m_goBeam.transform.Find("Beam").GetComponent<MeshRenderer>();
        switch (starStoneState)
        {
            case starStoneManager.starStones.Purple:
                beamMeshRen.material = GameUtilities.instance.materialPower;
                beamParticles.startColor = GameUtilities.instance.colourPurplePower;
                break;
            case starStoneManager.starStones.Orange:
                beamMeshRen.material = GameUtilities.instance.materialHeat;
                beamParticles.startColor = GameUtilities.instance.colourOrangeHeat;
                break;
            case starStoneManager.starStones.Blue:
                beamMeshRen.material = GameUtilities.instance.materialIce;
                beamParticles.startColor = GameUtilities.instance.colourBlueIce;
                break;
            case starStoneManager.starStones.Pink:
                beamMeshRen.material = GameUtilities.instance.materialHeal;
                beamParticles.startColor = GameUtilities.instance.colourPinkHeal;
                break;
            default:
                break;
        }

        //Position the beam so the visual matches the the weapon's range
        m_goBeam.transform.localPosition = Vector3.zero;
        m_goBeam.transform.localRotation = Quaternion.Euler(Vector3.zero);
        GameObject goBeamChild = m_goBeam.transform.Find("Beam").gameObject;
        goBeamChild.transform.localPosition = new Vector3(0f, 0f, (m_prototypeTemplate.GetRange() / 2f) - 0.5f);
    }

    private static float RemapNumber(float value, float lower, float upper, float newLower, float newUpper)
    {
        //Remaps a number between lower and upper to a corresponding value between newLower and newUpper
        return (value - lower) / (upper - lower) * (newUpper - newLower) + newLower;
    }
}