using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spell
{
    private float lastCast = 0f;
    private RawSpell rawSpell;

    private SpellCaster owner;

    public Spell(RawSpell rawSpell, SpellCaster owner)
    {
        this.rawSpell = rawSpell;
        this.owner = owner;
    }

    public bool IsReady()
    {
        return Time.time > lastCast + RPN.eval(rawSpell.CoolDown, owner.GetContext().ToDictionary());
    }

    public float LastCast() => lastCast;

    public string GetName() => rawSpell.Name;
    public int GetIcon() => rawSpell.Icon;

    public float GetDelay()
    {
        var ctx = owner.GetContext().ToDictionary();

        return RPN.eval(rawSpell.Delay ?? "0", ctx);
    }

    public float GetAngle()
    {
        var ctx = owner.GetContext().ToDictionary();

        return RPN.eval(rawSpell.Angle ?? "0", ctx);
    }

    public int GetManaCost()
    {
        var ctx = owner.GetContext().ToDictionary();

        float manaCost = RPN.eval(rawSpell.ManaCost, ctx);
        float manaAdder = RPN.eval(rawSpell.ManaAdder ?? "0", ctx);
        float manaMultiplier = RPN.eval(rawSpell.ManaMultiplier ?? "1", ctx);

        return Mathf.RoundToInt((manaCost + manaAdder) * manaMultiplier);
    }

    public float GetDamage()
    {
        var ctx = owner.GetContext().ToDictionary();

        float damage = RPN.eval(rawSpell.BaseDamage?.Amount ?? "0", ctx);
        float damageAdder = RPN.eval(rawSpell.DamageAdder ?? "0", ctx);
        float damageMultiplier= RPN.eval(rawSpell.DamageMultiplier ?? "1", ctx);

        return (damage + damageAdder) * damageMultiplier;
    }

    public float GetCoolDown()
    {
        var ctx = owner.GetContext().ToDictionary();

        float coolDown = RPN.eval(rawSpell.CoolDown ?? "0", ctx);
        float coolDownAdder = RPN.eval(rawSpell.CoolDownAdder ?? "0", ctx);
        float coolDownMultiplier = RPN.eval(rawSpell.CoolDownMultiplier ?? "1", ctx);

        return (coolDown + coolDownAdder) * coolDownMultiplier;
    }

    public float GetBaseProjectileSpeed()
    {
        var ctx = owner.GetContext().ToDictionary();

        float speed = RPN.eval(rawSpell.BaseProjectile?.Speed ?? "0", ctx);
        float speedAdder = RPN.eval(rawSpell.SpeedAdder ?? "0", ctx);
        float speedMultiplier = RPN.eval(rawSpell.SpeedMultiplier ?? "1", ctx);

        return (speed + speedAdder) * speedMultiplier;
    }

    public float GetSecondaryProjectileSpeed()
    {
        var ctx = owner.GetContext().ToDictionary();

        float speed = RPN.eval(rawSpell.SecondaryProjectile?.Speed ?? "0", ctx);
        float speedAdder = RPN.eval(rawSpell.SpeedAdder ?? "0", ctx);
        float speedMultiplier = RPN.eval(rawSpell.SpeedMultiplier ?? "1", ctx);

        return (speed + speedAdder) * speedMultiplier;
    }

    private void FireProjectiles(Vector3 where, Vector3 dir)
    {
        // fire base projectile

        string baseTrajectory = rawSpell.ProjectileTrajectory ?? rawSpell.BaseProjectile?.Trajectory;
        float baseSpeed = GetBaseProjectileSpeed();
        
        GameManager.Instance.projectileManager.CreateProjectile(
            rawSpell.Icon, baseTrajectory, where, dir, baseSpeed, OnHit);

        // fire secondary projectile if it exists

        string secondaryTrajectory = rawSpell.ProjectileTrajectory ?? rawSpell.SecondaryProjectile?.Trajectory;
        float secondarySpeed = GetSecondaryProjectileSpeed();
        
        GameManager.Instance.projectileManager.CreateProjectile(
            rawSpell.Icon, secondaryTrajectory, where, dir, secondarySpeed, OnHit);
    }

    private IEnumerator FireProjectilesWithDouble(Vector3 where, Vector3 dir)
    {
        FireProjectiles(where, dir);

        if (rawSpell.DoubleProjectile == true)
        {
            yield return new WaitForSeconds(GetDelay());

            FireProjectiles(where, dir);
        }
    }

    public IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        lastCast = Time.time;
        var ctx = owner.GetContext().ToDictionary();

        Vector3 dir = (target - where).normalized;

        if (rawSpell.SplitProjectile == true)
        {
            float angleOffset = GetAngle();
            Quaternion rotation1 = Quaternion.Euler(0, 0, angleOffset);
            Quaternion rotation2 = Quaternion.Euler(0, 0, -angleOffset);

            Vector3 dir1 = rotation1 * dir;
            Vector3 dir2 = rotation2 * dir;

            yield return FireProjectilesWithDouble(where, dir1);
            yield return FireProjectilesWithDouble(where, dir2);
        }
        else
        {
            yield return FireProjectilesWithDouble(where, dir);
        }

        yield return null;
    }

    public virtual void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != owner.team)
        {
            other.Damage(new Damage(Mathf.RoundToInt(GetDamage()), Damage.TypeFromString(rawSpell.BaseDamage?.Type)));
        }
    }
}
