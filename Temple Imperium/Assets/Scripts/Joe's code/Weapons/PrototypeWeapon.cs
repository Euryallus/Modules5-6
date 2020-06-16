using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeWeapon : Weapon
{
    public PrototypeWeaponTemplate m_prototypeTemplate { get; private set; }
    public float m_damageCharge { get; private set; }
    public bool m_charging { get; private set; }

    private float m_damageTimer;
    private GameObject m_goWeapon;
    private GameObject m_goBeam;

    public PrototypeWeapon(WeaponHolder weaponHolder, PrototypeWeaponTemplate template) : base(weaponHolder, template)
    {
        m_prototypeTemplate = template;
    }

    public override void SwitchingToThisWeapon()
    {
        m_charging = false;
    }

    public override void SwitchingToOtherWeapon()
    {
        m_charging = false;
        SoundEffectPlayer.instance.StopLoopingSoundEffect("protoBeam");
    }

    public override void HeldUpdate()
    {
        base.HeldUpdate();

        if (m_charging)
        {
            m_damageCharge += Time.deltaTime;
            m_damageTimer -= Time.deltaTime;
            if (m_damageCharge >= 1f)
            {
                m_damageCharge = 1f;
            }
        }

        if (m_goBeam != null)
        {
            float beamWidth;
            if (m_weaponHolder.generatorStates.returnState() == generatorStates.starStoneActive.Purple)
            {
                beamWidth = 0.4f;
            }
            else
            {
                beamWidth = (m_damageCharge * 0.4f);
            }
            m_goBeam.transform.Find("Beam").localScale = new Vector3(beamWidth, (m_prototypeTemplate.GetRange() / 2f) - 0.5f, beamWidth);
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Attack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {
        m_attackIntervalTimer = m_template.GetAttackInterval();
        m_goWeapon = weaponGameObject;

        generatorStates.starStoneActive generatorState = m_weaponHolder.generatorStates.returnState();

        if (!m_charging)
        {
            StartCharging(weaponGameObject);
            if (generatorState != generatorStates.starStoneActive.Purple)
                CreateBeamGameObject(weaponGameObject);
        }

        //TODO: Remove tempStarStoneState and do a proper check for active star stone
        switch (generatorState)
        {
            case generatorStates.starStoneActive.None:
                DefaultAttack(weaponAimInfo, weaponGameObject, prefabAttackLight, transformHead, buttonDown);
                break;
            case generatorStates.starStoneActive.Orange:
                HeatAttack(weaponAimInfo, weaponGameObject, prefabAttackLight, transformHead, buttonDown);
                break;
            case generatorStates.starStoneActive.Purple:
                PowerAttack(weaponAimInfo, weaponGameObject, prefabAttackLight, transformHead, buttonDown);
                break;
            case generatorStates.starStoneActive.Blue:
                IceAttack(weaponAimInfo, weaponGameObject, prefabAttackLight, transformHead, buttonDown);
                break;
            case generatorStates.starStoneActive.Pink:
                HealAttack(weaponAimInfo, weaponGameObject, prefabAttackLight, transformHead, buttonDown);
                break;
        }

        weaponGameObject.transform.Find("Weapon").GetComponent<Animator>().SetBool("Shooting", true);
    }

    private void DefaultAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {
        if (m_damageTimer <= 0)
        {
            m_damageTimer = m_prototypeTemplate.GetDamageInterval();

            if (weaponAimInfo.m_raycastHit)
            {
                Debug.Log("Proto weapon firing, hitting " + weaponAimInfo.m_hitInfo.transform.name);

                if (weaponAimInfo.m_hitInfo.collider.gameObject.CompareTag("Enemy"))
                {
                    float damagePerc = m_damageCharge / 1f;
                    int scaledDamage = Mathf.RoundToInt( RemapNumber(damagePerc, 0f, 1f, m_template.GetMinAttackDamage(), m_template.GetMaxAttackDamage()) );

                    weaponAimInfo.m_hitInfo.transform.GetComponent<Enemy>().Damage(scaledDamage);
                    UIManager.instance.ShowEnemyHitPopup(scaledDamage, weaponAimInfo.m_hitInfo.point);
                }
            }
            else
            {
                Debug.Log("Proto weapon firing, hitting nothing");
            }
        }
    }

    private void HeatAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {
        if (m_damageTimer <= 0)
        {
            m_damageTimer = m_prototypeTemplate.GetDamageInterval();

            if (weaponAimInfo.m_raycastHit)
            {
                Debug.Log("[HEAT] Proto weapon firing, hitting " + weaponAimInfo.m_hitInfo.transform.name);

                if (weaponAimInfo.m_hitInfo.collider.gameObject.CompareTag("Enemy"))
                {
                    float damagePerc = m_damageCharge / 1f;
                    int scaledDamage = Mathf.RoundToInt(RemapNumber(damagePerc, 0f, 1f, m_template.GetMinAttackDamage(), m_template.GetMaxAttackDamage()));

                    Enemy hitEnemy = weaponAimInfo.m_hitInfo.transform.GetComponent<Enemy>();
                    hitEnemy.Damage(scaledDamage);
                    hitEnemy.setOnFire(m_prototypeTemplate.GetFireEffectTime(), m_prototypeTemplate.GetFireDamage(), m_prototypeTemplate.GetTimeBetweenFireDamage());
                    UIManager.instance.ShowEnemyHitPopup(scaledDamage, weaponAimInfo.m_hitInfo.point);
                }
            }
            else
            {
                Debug.Log("[HEAT] Proto weapon firing, hitting nothing");
            }
        }
    }

    private void PowerAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {
        //Fully charged and ready to shoot
        if(m_damageCharge == 1f)
        {
            CreateBeamGameObject(weaponGameObject);

            if (weaponAimInfo.m_raycastHit)
            {
                Debug.Log("[POWER] Proto weapon firing, hitting " + weaponAimInfo.m_hitInfo.transform.name);

                if (weaponAimInfo.m_hitInfo.collider.gameObject.CompareTag("Enemy"))
                {
                    int damageAmount = Random.Range(m_prototypeTemplate.GetMinPowerDamage(), m_prototypeTemplate.GetMaxPowerDamage() + 1); ;

                    Enemy hitEnemy = weaponAimInfo.m_hitInfo.transform.GetComponent<Enemy>();
                    hitEnemy.Damage(damageAmount);
                    UIManager.instance.ShowEnemyHitPopup(damageAmount, weaponAimInfo.m_hitInfo.point);
                }
            }
            else
            {
                Debug.Log("[POWER] Proto weapon firing, hitting nothing");
            }

            SoundEffectPlayer.instance.PlaySoundEffect2D(m_prototypeTemplate.m_powerSound, m_prototypeTemplate.m_powerSoundVolume);
            SoundEffectPlayer.instance.StopLoopingSoundEffect("protoBeam");
            m_charging = false;
            m_attackIntervalTimer = m_template.GetAttackInterval();

            m_weaponHolder.DestroyWeaponGameObjectAfterTime(m_goBeam, 0.2f);
        }
    }

    private void IceAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {

    }

    private void HealAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {

    }

    private void StartCharging(GameObject weaponGameObject)
    {
        m_charging = true;
        m_damageCharge = 0f;
        m_damageTimer = 0f;

        SoundEffectPlayer.instance.PlaySoundEffect2D(m_template.GetAttackSound(), m_template.GetAttackSoundVolume(), 0.95f, 1.05f);
        SoundEffectPlayer.instance.PlayLoopingSoundEffect(m_prototypeTemplate.GetFiringSound(), false, Vector3.zero, "protoBeam", m_prototypeTemplate.GetFiringSoundVolume());
    }

    public void StopAttack(Transform transformHead)
    {
        m_charging = false;

        if(m_goBeam != null)
        {
            Object.Destroy(m_goBeam);
        }

        if(m_goWeapon != null)
        {
            m_goWeapon.transform.Find("Weapon").GetComponent<Animator>().SetBool("Shooting", false);
        }
        SoundEffectPlayer.instance.PlaySoundEffect2D(m_prototypeTemplate.GetDisableSound(), m_prototypeTemplate.GetDisableSoundVolume());
        SoundEffectPlayer.instance.StopLoopingSoundEffect("protoBeam");
    }

    private void CreateBeamGameObject(GameObject weaponGameObject)
    {
        //Create the beam and position it based on range
        m_goBeam = Object.Instantiate(m_prototypeTemplate.GetBeamGameObject(), weaponGameObject.transform.Find("AimPoint"));

        //Set material based on active StarStone
        MeshRenderer beamMeshRen = m_goBeam.transform.Find("Beam").GetComponent<MeshRenderer>();
        ParticleSystem.MainModule beamParticles = m_goBeam.transform.Find("Beam Particles").GetComponent<ParticleSystem>().main;
        generatorStates.starStoneActive starStoneState = m_weaponHolder.generatorStates.returnState();

        switch (starStoneState)
        {
            case generatorStates.starStoneActive.Purple:
                beamMeshRen.material = GameUtilities.instance.materialPower;
                beamParticles.startColor = GameUtilities.instance.colourPurplePower;
                break;
            case generatorStates.starStoneActive.Orange:
                beamMeshRen.material = GameUtilities.instance.materialHeat;
                beamParticles.startColor = GameUtilities.instance.colourOrangeHeat;
                break;
            case generatorStates.starStoneActive.Blue:
                beamMeshRen.material = GameUtilities.instance.materialIce;
                beamParticles.startColor = GameUtilities.instance.colourBlueIce;
                break;
            case generatorStates.starStoneActive.Pink:
                beamMeshRen.material = GameUtilities.instance.materialHeal;
                beamParticles.startColor = GameUtilities.instance.colourPinkHeal;
                break;
            default:
                break;
        }

        m_goBeam.transform.localPosition = Vector3.zero;
        m_goBeam.transform.localRotation = Quaternion.Euler(Vector3.zero);
        GameObject goBeamChild = m_goBeam.transform.Find("Beam").gameObject;
        goBeamChild.transform.localPosition = new Vector3(0f, 0f, (m_prototypeTemplate.GetRange() / 2f) - 0.5f);
    }

    private static float RemapNumber(float value, float lower, float upper, float newLower, float newUpper)
    {
        return (value - lower) / (upper - lower) * (newUpper - newLower) + newLower;
    }
}
