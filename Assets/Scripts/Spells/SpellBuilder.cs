using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;


public class SpellBuilder
{
    private System.Random random = new System.Random();
    private List<string> baseSpellKeys = new List<string>();
    private List<string> modifierSpellKeys = new List<string>();

    public SpellBuilder()
    {
        foreach (var pair in SpellManager.Instance.AllSpells)
        {
            if (pair.Value.ContainsKey("damage"))  // crude but effective split
                baseSpellKeys.Add(pair.Key);
            else
                modifierSpellKeys.Add(pair.Key);
        }
    }

    public Spell Build(SpellCaster owner)
    {
        return CreateRandomSpell(owner);
    }

    private Spell CreateRandomSpell(SpellCaster owner)
    {
        string baseKey = baseSpellKeys[random.Next(baseSpellKeys.Count)];
        Spell baseSpell = CreateBaseSpell(owner, baseKey);

        int numModifiers = random.Next(0, 3);
        for (int i = 0; i < numModifiers; i++)
        {
            string modKey = modifierSpellKeys[random.Next(modifierSpellKeys.Count)];
            baseSpell = CreateModifierSpell(owner, modKey, baseSpell);
        }

        return baseSpell;
    }

    private Spell CreateBaseSpell(SpellCaster owner, string key)
    {
        JObject spellObj = SpellManager.Instance.AllSpells[key];

        switch (key)
        {
            case "arcane_bolt":
                return new ArcaneBoltSpell(owner, spellObj);
            case "magic_missile":
                return new MagicMissileSpell(owner, spellObj);
            case "arcane_blast":
                return new ArcaneBlastSpell(owner, spellObj);
            case "arcane_spray":
                return new ArcaneSpraySpell(owner, spellObj);
            default:
                Debug.LogWarning("Unknown base spell key: " + key);
                return new ArcaneBoltSpell(owner, spellObj);
        }
    }

    private Spell CreateModifierSpell(SpellCaster owner, string key, Spell inner)
    {
        JObject modObj = SpellManager.Instance.AllSpells[key];
        return new ModifierSpell(owner, inner, modObj);  // you'll customize this
    }
}
