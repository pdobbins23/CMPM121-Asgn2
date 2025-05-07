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

    public string GetName() => rawSpell.Name;
    public int GetIcon() => rawSpell.Icon;

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

        float speed = RPN.eval(rawSpell.SecondaryProjectile.Speed ?? "0", ctx);
        float speedAdder = RPN.eval(rawSpell.SpeedAdder ?? "0", ctx);
        float speedMultiplier = RPN.eval(rawSpell.SpeedMultiplier ?? "1", ctx);

        return (speed + speedAdder) * speedMultiplier;
    }

    public IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        lastCast = Time.time;
        var ctx = owner.GetContext().ToDictionary();

        Vector3 direction = (target - where).normalized;

        string trajectory = rawSpell.ProjectileTrajectory ?? rawSpell.Projectile.Trajectory;
        float speed = GetProjectileSpeed();

        if (rawSpell.DoubleProjectile)
        {
            GameManager.Instance.projectileManager.CreateProjectile(
                rawSpell.Icon, trajectory, where, direction, speed, OnHit);

            yield return new WaitForSeconds(rawSpell.Delay);

            GameManager.Instance.projectileManager.CreateProjectile(
                rawSpell.Icon, trajectory, where, direction, speed, OnHit);
        }
        else if (rawSpell.SplitProjectile)
        {
            float angleOffset = mods.angle;
            Quaternion rotation1 = Quaternion.Euler(0, 0, angleOffset);
            Quaternion rotation2 = Quaternion.Euler(0, 0, -angleOffset);

            Vector3 dir1 = rotation1 * direction;
            Vector3 dir2 = rotation2 * direction;

            GameManager.Instance.projectileManager.CreateProjectile(icon, trajectory, where, dir1, speed, OnHit);
            GameManager.Instance.projectileManager.CreateProjectile(icon, trajectory, where, dir2, speed, OnHit);
        }
        else
        {
            GameManager.Instance.projectileManager.CreateProjectile(icon, trajectory, where, direction, speed, OnHit);
        }

        yield return null;
    }

    public virtual void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != owner.team)
        {
            other.Damage(new Damage(Mathf.RoundToInt(GetDamage()), Damage.TypeFromString(rawSpell.Damage.Type)));
        }
    }
}
