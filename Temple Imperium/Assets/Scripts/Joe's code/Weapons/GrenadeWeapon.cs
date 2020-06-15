using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrenadeWeapon : Weapon
{
    public GrenadeWeaponTemplate m_grenadeTemplate { get; private set; }
    public int m_grenadeCount { get; private set; }

    private GameObject m_goThrow;
    private List<ThrownGrenade> m_thrownGrenades = new List<ThrownGrenade>();

    public GrenadeWeapon(WeaponHolder weaponHolder, GrenadeWeaponTemplate template) : base(weaponHolder, template)
    {
        m_grenadeTemplate = template;
        m_grenadeCount = template.GetStartCount();
    }

    public override bool ReadyToFire()
    {
        if(base.ReadyToFire() && m_grenadeCount > 0)
        {
            return true;
        }
        return false;
    }

    //Attack is called when the grenade is thrown
    public override void Attack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {
        m_grenadeCount--;

        m_attackIntervalTimer = m_template.GetAttackInterval();

        SetHideHeldWeapon(true);
        m_weaponHolder.SetHeldWeaponHidden(m_hideHeldWeapon);

        m_goThrow = Object.Instantiate(m_grenadeTemplate.GetThrowGameObject(), transformHead);
        m_goThrow.transform.localPosition += new Vector3(0f, 0f, m_template.GetVisualOffset().z);
        m_goThrow.transform.SetParent(null);
        m_goThrow.GetComponent<Rigidbody>().velocity = transformHead.forward * m_grenadeTemplate.GetThrowVelocity();

        m_thrownGrenades.Add(new ThrownGrenade(this, m_goThrow, m_grenadeTemplate.GetDelay(), transformHead.position));

        SoundEffectPlayer.instance.PlaySoundEffect2D(m_grenadeTemplate.GetThrowSound(), 1f, 0.95f, 1.05f);
    }

    public override void Update()
    {
        base.Update();

        for (int i = 0; i < m_thrownGrenades.Count; i++)
        {
            m_thrownGrenades[i].Update();
        }
    }

    public override void HeldUpdate()
    {
        base.HeldUpdate();

        if (Input.GetKeyDown(KeyCode.G))
        {
            m_grenadeCount++;
        }


        if(m_attackIntervalTimer <= 0)
        {
            if (m_hideHeldWeapon && m_grenadeCount > 0)
            {
                SetHideHeldWeapon(false);
            }
        }
    }

    public void RemoveThrownGrenade(ThrownGrenade thrownGrenade)
    {
        m_thrownGrenades.Remove(thrownGrenade);
    }
}

public class ThrownGrenade
{
    private GameObject m_goThrow;
    private float m_delayTimer;
    private Vector3 m_headPosition;
    private GrenadeWeapon m_grenadeParent;

    public ThrownGrenade(GrenadeWeapon grenadeParent, GameObject goThrow, float delay, Vector3 headPosition)
    {
        m_grenadeParent = grenadeParent;
        m_goThrow = goThrow;
        m_delayTimer = delay;
        m_headPosition = headPosition;
    }

    public void Update()
    {
        if (m_delayTimer >= 0)
        {
            m_delayTimer -= Time.deltaTime;
        }
        else
        {
            Explode();
        }
    }

    private void Explode()
    {
        Vector3 impactPos = m_goThrow.transform.position;

        GameObject explosionParticles = m_grenadeParent.m_grenadeTemplate.GetExplosionParticles();
        if (explosionParticles != null)
        {
            Object.Instantiate(explosionParticles, impactPos, Quaternion.identity);
        }

        float explosionRadius = m_grenadeParent.m_grenadeTemplate.GetImpactRadius();
        float fragmentRadius = m_grenadeParent.m_grenadeTemplate.GetFragRadius();

        Collider[] collidersInExplosionRad = Physics.OverlapSphere(impactPos, explosionRadius);
        Collider[] collidersInFragRad = Physics.OverlapSphere(impactPos, fragmentRadius);

        for (int i = 0; i < collidersInFragRad.Length; i++)
        {
            Rigidbody rigidbody = collidersInFragRad[i].GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                //Colliders in explosion range
                if (collidersInExplosionRad.Contains(collidersInFragRad[i]))
                {
                    if (rigidbody.gameObject.CompareTag("Enemy"))
                    {
                        int damageAmount = Random.Range(m_grenadeParent.m_template.GetMinAttackDamage(), m_grenadeParent.m_template.GetMaxAttackDamage() + 1);
                        rigidbody.gameObject.GetComponent<Enemy>().Damage(damageAmount);
                        UIManager.instance.ShowEnemyHitPopup(damageAmount, rigidbody.gameObject.transform.position);
                    }
                    rigidbody.AddExplosionForce(600f, impactPos, explosionRadius);
                }
                //Colliders in frag range
                else
                {
                    if (rigidbody.gameObject.CompareTag("Enemy"))
                    {
                        int damageAmount = m_grenadeParent.m_grenadeTemplate.GetFragDamage();
                        rigidbody.gameObject.GetComponent<Enemy>().Damage(damageAmount);
                        UIManager.instance.ShowEnemyHitPopup(damageAmount, rigidbody.gameObject.transform.position);
                    }
                }
            }
        }

        SoundEffectPlayer.instance.PlaySoundEffect3D(m_grenadeParent.m_template.GetAttackSound(), m_goThrow.transform.position, 1f, 0.95f, 1.05f);

        Object.Destroy(m_goThrow);

        m_grenadeParent.RemoveThrownGrenade(this);
    }
}
