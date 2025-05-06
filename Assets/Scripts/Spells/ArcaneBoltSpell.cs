using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ArcaneBoltSpell : Spell
{
    private float lastCast;
    private float cooldown;
    private int manaCost;
    private int damage;
    private float speed;
    private int icon;
    private Hittable.Team team;

    public ArcaneBoltSpell(SpellCaster owner, JObject data) : base(owner)
    {
        var ctx = owner.GetContext().ToDictionary();
        damage = (int)RPN.Evaluate(data["damage"]["amount"].ToString(), ctx);
        manaCost = (int)RPN.Evaluate(data["mana_cost"].ToString(), ctx);
        cooldown = float.Parse(data["cooldown"].ToString());
        speed = RPN.Evaluate(data["projectile"]["speed"].ToString(), ctx);
        icon = data["icon"]?.ToObject<int>() ?? 0;
    }

    public override string GetName() => "Arcane Bolt";
    public override int GetManaCost() => manaCost;
    public override int GetDamage() => damage;
    public override float GetCooldown() => cooldown;
    public override int GetIcon() => icon;
    public override bool IsReady() => Time.time > lastCast + cooldown;

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        this.team = team;
        lastCast = Time.time;
        GameManager.Instance.projectileManager.CreateProjectile(icon, "straight", where, target - where, speed, OnHit);
        yield return null;
    }

    void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(damage, Damage.Type.ARCANE));
        }
    }
}
