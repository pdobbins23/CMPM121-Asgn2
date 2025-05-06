using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ModifierSpell : Spell
{
    private Spell innerSpell;
    private JObject data;

    public ModifierSpell(SpellCaster owner, Spell inner, JObject data) : base(owner)
    {
        innerSpell = inner;
        this.data = data;
    }

    public override string GetName() => $"{data["name"]?.ToString()} {innerSpell.name}";
    public override int GetManaCost(SpellContext ctx)
    {
        string rpnManaMultiplier = data["mana_multipler"]?.ToObject<string>() ?? "1";
        return Mathf.RoundToInt(innerSpell.GetManaCost() * RPN.eval(rpnManaMultiplier, ctx));
    }

    public override int GetDamage()
    {
        string rpnCoolDownMultiplier = data["cooldown_multiplier"]?.ToObject<string>() ?? "1";
        return Mathf.RoundToInt(innerSpell.GetManaCost() * RPN.eval(rpnCoolDownMultiplier, ctx));
    }

    public override float GetCooldown()
    {
        string rpnCoolDownMultiplier = data["cooldown_multipler"]?.ToObject<string>() ?? "1";
        return Mathf.RoundToInt(innerSpell.GetCoolDown() * RPN.eval(rpnCoolDownMultiplier, ctx));
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, SpellContext ctx, Modifiers mods)
    {
        mods.doubleProjectile = data["double_projectile"]?.ToObject<bool>() ?? false;
        mods.splitProjectile = data["split_projectile"]?.ToObject<bool>() ?? false;
        mods.projectileTrajectory = data["projectile_trajectory"]?.ToObject<string>() ?? "straight";
        
        string rpnDamageAdder = data["damage_adder"]?.ToObject<string>() ?? "1";
        string rpnSpeedMultiplier = data["speed_adder"]?.ToObject<string>() ?? "1";
        string rpnManaAdder = data["mana_adder"]?.ToObject<string>() ?? "1";
        string rpnCoolDownAdder = data["cooldown_adder"]?.ToObject<string>() ?? "1";

        string rpnDamageMultiplier = data["damage_multipler"]?.ToObject<string>() ?? "1";
        string rpnSpeedMultiplier = data["speed_multipler"]?.ToObject<string>() ?? "1";
        string rpnManaMultiplier = data["mana_multipler"]?.ToObject<string>() ?? "1";
        string rpnCoolDownMultiplier = data["cooldown_multipler"]?.ToObject<string>() ?? "1";
        
        string rpnDelay = data["delay"]?.ToObject<string>() ?? "0";
        string rpnAngle = data["angle"]?.ToObject<string>() ?? "0";

        mods.damageAdder = RPN.eval(rpnDamageAdder, ctx);
        mods.speedAdder = RPN.eval(rpnSpeedAdder, ctx);
        mods.manaAdder = RPN.eval(rpnManaAdder, ctx);
        mods.coolDownAdder = RPN.eval(rpnCoolDownAdder, ctx);

        mods.damageMultiplier = RPN.eval(rpnDamageMultiplier, ctx);
        mods.speedMultiplier = RPN.eval(rpnSpeedMultiplier, ctx);
        mods.manaMultiplier = RPN.eval(rpnManaMultiplier, ctx);
        mods.coolDownMultiplier = RPN.eval(rpnCoolDownMultiplier, ctx);

        mods.delay = RPN.eval(rpnDelay, ctx);
        mods.angle= RPN.eval(rpnAngle, ctx);

        return innerSpell.Cast(where, target, team, ctx, mods);
    }

    public override void OnHit(Hittable other, Vector3 impact) {
        innerSpell.OnHit(other, impact);
    }
}
