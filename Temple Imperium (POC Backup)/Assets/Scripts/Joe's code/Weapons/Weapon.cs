using UnityEngine;

public abstract class Weapon
{
    public WeaponHolder m_weaponHolder { get; private set; }            //The weapon holder on the entity that is holding this weapon
    public WeaponTemplate m_template { get; private set; }              //The template that defines the weapon's base properties
    public bool m_hideHeldWeapon { get; protected set; }                //True if this weapon should not be visible (i.e. the holder has an empty hand)
    public float m_attackIntervalTimer { get; protected set; }          //Amount of time until an attack can be triggered
    public float m_alternateAttackIntervalTimer { get; protected set; } //Amount of time until a secondary attack can be triggered

    //Constructor
    public Weapon(WeaponHolder weaponHolder, WeaponTemplate template)
    {
        m_weaponHolder = weaponHolder;
        m_template = template;
    }

    //Called when an attack/alternate attack is triggered from a WeaponHolder
    public abstract void Attack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown);
    public abstract void AlternateAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, Transform transformHead);

    //Called when switching to/from this weapon in a WeaponHolder
    public virtual void SwitchingToThisWeapon() { }
    public virtual void SwitchingToOtherWeapon() { }

    //Weapons can only be used if the attack interval timer has reached 0 to prevent continuous attacks
    public virtual bool ReadyToAttack()
    {
        if(m_attackIntervalTimer <= 0)
        {
            return true;
        }
        return false;
    }
    //Same as above but for the alternate/secondary attack
    public virtual bool ReadyToAttackAlternate()
    {
        if (m_alternateAttackIntervalTimer <= 0)
        {
            return true;
        }
        return false;
    }

    protected void SetHideHeldWeapon(bool hideHeldWeapon)
    {
        m_hideHeldWeapon = hideHeldWeapon;
        m_weaponHolder.SetHeldWeaponHidden(hideHeldWeapon);
    }

    //Called every frame when this weapon has been added to a WeaponHolder
    public virtual void Update()
    {
        //Decrease the attack interval timers until they reach 0 (meaning ready to attack),
        //  using deltaTime so the timer is framerate-independent
        if (m_attackIntervalTimer > 0f)
        {
            m_attackIntervalTimer -= Time.deltaTime;
        }
        if (m_alternateAttackIntervalTimer > 0f)
        {
            m_alternateAttackIntervalTimer -= Time.deltaTime;
        }
    }

    //Called only on frames when this weapon is actively being held in a WeaponHolder
    public virtual void HeldUpdate() { }
}
