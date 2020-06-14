using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeWeapon : Weapon
{
    public PrototypeWeaponTemplate m_prototypeTemplate { get; private set; }

    private bool m_beamOn;
    private float m_damageCharge;
    private float m_damageTimer;
    private GameObject m_goWeapon;
    private GameObject m_goBeam;

    private string tempStarStoneState = "default";

    public PrototypeWeapon(WeaponHolder weaponHolder, PrototypeWeaponTemplate template) : base(weaponHolder, template)
    {
        m_prototypeTemplate = template;
    }

    public override void HeldUpdate()
    {
        base.HeldUpdate();

        if (m_beamOn)
        {
            m_damageCharge += Time.deltaTime;
            m_damageTimer -= Time.deltaTime;
            if (m_damageCharge >= 1f)
            {
                m_damageCharge = 1f;
            }
            if(m_goBeam != null)
            {
                float beamWidth = (m_damageCharge * 0.4f);
                m_goBeam.transform.Find("Beam").localScale = new Vector3(beamWidth, m_prototypeTemplate.GetRange(), beamWidth);
            }
        }
    }

    public override void Update()
    {
        base.Update();
    }

    //public override bool ReadyToFire()
    //{
    //    if(base.ReadyToFire() && !m_beamOn)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    public override void Attack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {
        m_attackIntervalTimer = m_template.GetAttackInterval();
        m_goWeapon = weaponGameObject;

        //TODO: Remove tempStarStoneState and do a proper check for active star stone
        switch (tempStarStoneState)
        {
            case "default":
                DefaultAttack(weaponAimInfo, weaponGameObject, prefabAttackLight, transformHead, buttonDown);
                break;
        }

        weaponGameObject.transform.Find("Weapon").GetComponent<Animator>().SetBool("Shooting", true);
    }

    private void DefaultAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {
        if (!m_beamOn)
        {
            m_beamOn = true;
            m_damageCharge = 0f;
            m_damageTimer = 0f;

            //Create the beam and position it based on range
            m_goBeam = Object.Instantiate(m_prototypeTemplate.GetBeamGameObject(), weaponGameObject.transform.Find("AimPoint"));
            m_goBeam.transform.localPosition = Vector3.zero;
            m_goBeam.transform.localRotation = Quaternion.Euler(Vector3.zero);
            GameObject goBeamChild = m_goBeam.transform.Find("Beam").gameObject;
            goBeamChild.transform.localPosition = new Vector3(0f, 0f, m_prototypeTemplate.GetRange());

            SoundEffectPlayer.instance.PlaySoundEffect(m_template.GetAttackSound(), true, transformHead.position, 1f, 0.95f, 1.05f);
        }

        if(m_damageTimer <= 0)
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

    public void DisableBeam(Transform transformHead)
    {
        m_beamOn = false;

        if(m_goBeam != null)
        {
            Object.Destroy(m_goBeam);
        }

        if(m_goWeapon != null)
        {
            m_goWeapon.transform.Find("Weapon").GetComponent<Animator>().SetBool("Shooting", false);
        }
        SoundEffectPlayer.instance.PlaySoundEffect(m_prototypeTemplate.GetDisableSound(), true, transformHead.position, 1f, 0.95f, 1.05f);
    }

    private float RemapNumber(float value, float lower, float upper, float newLower, float newUpper)
    {
        return (value - lower) / (upper - lower) * (newUpper - newLower) + newLower;
    }
}
