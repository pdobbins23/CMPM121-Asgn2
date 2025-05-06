using UnityEngine;
using System.Collections;

public abstract class BaseSpell
{
    protected SpellCaster owner;
    protected Hittable.Team team;
    protected float lastCast;

    public BaseSpell(SpellCaster owner)
    {
        this.owner = owner;
    }

    public abstract string GetName();
    public abstract int GetManaCost();
    public abstract int GetDamage();
    public abstract float GetCooldown();
    public abstract int GetIcon();

    public bool IsReady()
    {
        return Time.time > lastCast + GetCooldown();
    }

    public IEnumerator TryCast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        if (!IsReady()) yield break;
        if (owner.mana < GetManaCost()) yield break;

        owner.mana -= GetManaCost();
        lastCast = Time.time;
        this.team = team;

        yield return Cast(where, target);
    }

    protected abstract IEnumerator Cast(Vector3 where, Vector3 target);
}
