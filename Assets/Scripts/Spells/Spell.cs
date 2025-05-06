using UnityEngine;
using System.Collections;

public class Spell
{
    struct Modifiers {
        bool doubleProjectile = false;
        bool splitProjectile = false;
        string projectileTrajectory = "straight";

        float damageAdder = 0;
        float speedAdder = 0;
        float manaAdder = 0;
        float coolDownAdder = 0;

        float damageMultiplier = 1;
        float speedMultiplier = 1;
        float manaMultiplier = 1;
        float coolDownMultiplier = 1;

        float delay = 0;
        float angle = 0;
    }
    
    struct Damage {
        string amount;
        string type;
    }

    struct Projectile {
        string trajectory;
        string speed;
        int sprite;
        string lifetime;
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
        return Time.time > lastCast + coolDown;
    }

    public int GetManaCost(SpellContext ctx) => RPN.eval(manaCost, ctx);
    public float GetDamage(SpellContext ctx) => RPN.eval(damage.amount, ctx);
    public float GetSecondaryDamage(SpellContext ctx) => RPN.eval(secondaryDamage.amount, ctx);
    public int GetCount(SpellContext ctx) => RPN.eval(count, ctx);
    public float GetCoolDown(SpellContext ctx) => RPN.eval(coolDown, ctx);
    public float GetProjectileSpeed(SpellContext ctx) => RPN.eval(projectile.speed, ctx);
    public float GetProjectileLifetime(SpellContext ctx) => RPN.eval(projectile.lifetime, ctx);
    public float GetSecondaryProjectileSpeed(SpellContext ctx) => RPN.eval(secondaryProjectile.speed, ctx);
    public float GetSecondaryProjectileLifetime(SpellContext ctx) => RPN.eval(secondaryProjectile.lifetime, ctx);

    public IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, SpellContext context, Modifiers mods)
    {
        if (!IsReady()) yield break;

        var ctx = context.ToDictionary();

        float manaCost = RPN.eval(manaCost, ctx);
        
        if (owner.mana < manaCost) yield break;

        owner.mana -= manaCost;
        lastCast = Time.time;
        this.team = team;

        // TODO: Implement all spell logic

        float projectileSpeed = RPN.eval(projectile.speed, ctx);

        GameManager.Instance.projectileManager.CreateProjectile(icon, projectile.trajectory, where, target - where, projectileSpeed, OnHit);
    }

    public void OnHit(Hittable other, Vector3 impact) {
        // TODO
        if (other.team != team)
        {
            // other.Damage(new Damage(damage, Damage.Type.ARCANE));
        }
    }

    public void OnSecondaryHit(Hittable other, Vector3 impact) {
        // TODO
        if (other.team != team)
        {
            // other.Damage(new Damage(damage, Damage.Type.ARCANE));
        }
    }
}
