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

    public override string GetName() => $"{data["name"]?.ToString()} {innerSpell.GetName()}";
    public override int GetManaCost()
    {
        float multiplier = data["mana_multiplier"]?.ToObject<float>() ?? 1f;
        return Mathf.RoundToInt(innerSpell.GetManaCost() * multiplier);
    }

    public override int GetDamage()
    {
        float multiplier = data["damage_multiplier"]?.ToObject<float>() ?? 1f;
        return Mathf.RoundToInt(innerSpell.GetDamage() * multiplier);
    }

    public override float GetCooldown()
    {
        float multiplier = data["cooldown_multiplier"]?.ToObject<float>() ?? 1f;
        return innerSpell.GetCooldown() * multiplier;
    }

    public override int GetIcon() => innerSpell.GetIcon();
    public override bool IsReady() => innerSpell.IsReady();

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        return innerSpell.Cast(where, target, team); // extend later for behavior
    }
}
