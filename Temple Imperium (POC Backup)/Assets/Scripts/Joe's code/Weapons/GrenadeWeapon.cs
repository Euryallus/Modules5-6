using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//------------------------------------------------------\\
//  A grenade that can be used by an entity with        \\
//  a WeaponHolder that holds a GrenadeWeaponTemplate.  \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class GrenadeWeapon : Weapon
{
    //Properties
    public GrenadeWeaponTemplate m_grenadeTemplate { get; private set; }    //The template that defines the grenade's base properties
    public int m_grenadeCount { get; private set; }                         //The number of grenades that can be thrown before running out

    private List<ThrownGrenade> m_thrownGrenades = new List<ThrownGrenade>();   //All grenades that have been thrown and are active in the scene

    //Constructor
    public GrenadeWeapon(WeaponHolder weaponHolder, GrenadeWeaponTemplate template) : base(weaponHolder, template)
    {
        m_grenadeTemplate = template;
        m_grenadeCount = template.GetStartCount();
    }

    public override bool ReadyToAttack()
    {
        //Only allow an attack if all grenades have not been used
        if(base.ReadyToAttack() && m_grenadeCount > 0)
        {
            return true;
        }
        return false;
    }

    //Attack is called when the grenade is thrown
    public override void Attack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {
        //Use up a grenade and reset attack interval to prevent continuous throwing
        m_grenadeCount--;
        m_attackIntervalTimer = m_template.GetAttackInterval();

        //Hide the held grenade to give the appearance of it being thrown
        SetHideHeldWeapon(true);
        m_weaponHolder.SetHeldWeaponHidden(m_hideHeldWeapon);

        //Add a new ThrownGrenade - thrown grenades are handled seperately to allow multiple thrown grenades to be active at any time,
        //  while still only having a single grenade slot in the weapon holder
        m_thrownGrenades.Add(new ThrownGrenade(this, m_grenadeTemplate.GetDelay(), transformHead));

        //Play the throw sound set in the template editor
        SoundEffectPlayer.instance.PlaySoundEffect2D(m_grenadeTemplate.m_throwSound, m_grenadeTemplate.m_throwSoundVolume, 0.95f, 1.05f);
    }

    //Grenade has no alternate attack
    public override void AlternateAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, Transform transformHead) {}

    public void RemoveThrownGrenade(ThrownGrenade thrownGrenade)
    {
        m_thrownGrenades.Remove(thrownGrenade);
    }

    public override void Update()
    {
        //Update this weapon and all thrown grenades
        base.Update();
        for (int i = 0; i < m_thrownGrenades.Count; i++)
        {
            m_thrownGrenades[i].Update();
        }
    }

    public override void HeldUpdate()
    {
        base.HeldUpdate();

        //Debug shortcut for adding more grenades
        if (Input.GetKeyDown(KeyCode.G))
        {
            m_grenadeCount++;
        }

        //Show the held grenade if ready to attack
        if(m_attackIntervalTimer <= 0)
        {
            if (m_hideHeldWeapon && m_grenadeCount > 0)
            {
                SetHideHeldWeapon(false);
            }
        }
    }
}

public class ThrownGrenade
{
    //Properties
    private GameObject m_goThrow;
    private float m_delayTimer;
    private GrenadeWeapon m_grenadeParent;

    //Constructor
    public ThrownGrenade(GrenadeWeapon grenadeParent, float delay, Transform transformHead)
    {
        m_grenadeParent = grenadeParent;
        m_delayTimer = delay;

        //Create the GameObject for this therown grenade and position it just in front of the player to start
        m_goThrow = Object.Instantiate(grenadeParent.m_grenadeTemplate.GetThrowGameObject(), transformHead);
        m_goThrow.transform.localPosition += new Vector3(0f, 0f, grenadeParent.m_template.GetVisualOffset().z);
        m_goThrow.transform.SetParent(null);
        //Give it a velocity based on the value set in the weapon's template
        m_goThrow.GetComponent<Rigidbody>().velocity = transformHead.forward * grenadeParent.m_grenadeTemplate.GetThrowVelocity();
    }

    public void Update()
    {
        //Reduce the delay timer until it reaches 0, then explode the grenade
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

        //Explosion particle effect
        GameObject explosionParticles = m_grenadeParent.m_grenadeTemplate.GetExplosionParticles();
        if (explosionParticles != null)
        {
            Object.Instantiate(explosionParticles, impactPos, Quaternion.identity);
        }
        //Explosion damage radii set in the weapon template editor
        float explosionRadius = m_grenadeParent.m_grenadeTemplate.GetImpactRadius();
        float fragmentRadius = m_grenadeParent.m_grenadeTemplate.GetFragRadius();

        //Find all colliders in the explosion and fragment radii
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
                    //Apply explosion damage to any hit enemies
                    if (rigidbody.gameObject.CompareTag("Enemy"))
                    {
                        int damageAmount = Random.Range(m_grenadeParent.m_template.GetMinAttackDamage(), m_grenadeParent.m_template.GetMaxAttackDamage() + 1);
                        rigidbody.gameObject.GetComponent<Enemy>().Damage(damageAmount);
                        UIManager.instance.ShowEnemyHitPopup(damageAmount, rigidbody.gameObject.transform.position);
                    }
                    //Add ad explosion force to any hit rigidbodies
                    rigidbody.AddExplosionForce(600f, impactPos, explosionRadius);
                }
                //Colliders in frag range
                else
                {
                    //Apply frag damage to any hit enemies
                    if (rigidbody.gameObject.CompareTag("Enemy"))
                    {
                        int damageAmount = m_grenadeParent.m_grenadeTemplate.GetFragDamage();
                        rigidbody.gameObject.GetComponent<Enemy>().Damage(damageAmount);
                        UIManager.instance.ShowEnemyHitPopup(damageAmount, rigidbody.gameObject.transform.position);
                    }
                }
                //Colliders in either range
                if (collidersInFragRad[i].gameObject.CompareTag("ExplodeOnImpact"))
                {
                    //Explode any hit objects with the ExplodeOnImpact component
                    collidersInFragRad[i].gameObject.GetComponent<ExplodeOnImpact>().Explode();
                }
            }
        }

        //Play the attack/explosion sound
        SoundEffectPlayer.instance.PlaySoundEffect3D(m_grenadeParent.m_template.m_attackSound, m_goThrow.transform.position, m_grenadeParent.m_template.m_attackSoundVolume, 0.95f, 1.05f);
        //Remove the thrown grenade GameObject/instantiated object
        Object.Destroy(m_goThrow);
        m_grenadeParent.RemoveThrownGrenade(this);
    }
}
