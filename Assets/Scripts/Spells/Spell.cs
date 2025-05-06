using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spell
{
    public struct Modifiers
    {
        public bool doubleProjectile;
        public bool splitProjectile;
        public string projectileTrajectory;

        public float damageAdder;
        public float speedAdder;
        public float manaAdder;
        public float coolDownAdder;

        public float damageMultiplier;
        public float speedMultiplier;
        public float manaMultiplier;
        public float coolDownMultiplier;

        public float delay;
        public float angle;
    }

    public struct Damage
    {
        public string amount;
        public string type;
    }

    public struct Projectile
    {
        public string trajectory;
        public string speed;
        public int sprite;
        public string lifetime;
    }

    public SpellCaster owner;
    public Hittable.Team team;
    public float lastCast;

    public string name;
    public string manaCost;
    public Damage damage;
    public Damage secondaryDamage;
    public string coolDown;
    public int icon;
    public string count;
    public Projectile projectile;
    public Projectile secondaryProjectile;

    public Spell(SpellCaster owner)
    {
        this.owner = owner;
    }

    public bool IsReady()
    {
        return Time.time > lastCast + RPN.eval(coolDown, owner.GetContext().ToDictionary());
    }

    // Evaluation helpers with context and modifiers
    public int GetManaCost(SpellContext ctx)
    {
        float baseCost = RPN.eval(manaCost, ctx.ToDictionary());
        return Mathf.RoundToInt((baseCost + ctx.Mods.manaAdder) * ctx.Mods.manaMultiplier);
    }

    public float GetDamage(SpellContext ctx)
    {
        float baseDmg = RPN.eval(damage.amount, ctx.ToDictionary());
        return (baseDmg + ctx.Mods.damageAdder) * ctx.Mods.damageMultiplier;
    }

    public float GetCooldown(SpellContext ctx)
    {
        float baseCD = RPN.eval(coolDown, ctx.ToDictionary());
        return (baseCD + ctx.Mods.coolDownAdder) * ctx.Mods.coolDownMultiplier;
    }

    public float GetProjectileSpeed(SpellContext ctx)
    {
        float baseSpeed = RPN.eval(projectile.speed, ctx.ToDictionary());
        return (baseSpeed + ctx.Mods.speedAdder) * ctx.Mods.speedMultiplier;
    }

    public IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, SpellContext ctx, Modifiers mods)
    {
        ctx.Mods = mods;

        if (!IsReady()) yield break;

        float cost = GetManaCost(ctx);
        if (owner.mana < cost) yield break;

        owner.mana -= Mathf.RoundToInt(cost);
        lastCast = Time.time;
        this.team = team;

        Vector3 direction = (target - where).normalized;

        string trajectory = mods.projectileTrajectory ?? projectile.trajectory;
        float speed = GetProjectileSpeed(ctx);

        if (mods.doubleProjectile)
        {
            GameManager.Instance.projectileManager.CreateProjectile(
                icon, trajectory, where, direction, speed, OnHit);
            yield return new WaitForSeconds(mods.delay);
            GameManager.Instance.projectileManager.CreateProjectile(
                icon, trajectory, where, direction, speed, OnHit);
        }
        else if (mods.splitProjectile)
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
        if (other.team != team)
        {
            float damageValue = GetDamage(owner.GetContext());
            other.Damage(new DamageStruct(Mathf.RoundToInt(damageValue), Damage.Type.ARCANE)); // replace as needed
        }
    }

    public virtual void OnSecondaryHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            float damageValue = RPN.eval(secondaryDamage.amount, owner.GetContext().ToDictionary());
            other.Damage(new DamageStruct(Mathf.RoundToInt(damageValue), Damage.Type.ARCANE));
        }
    }
}
