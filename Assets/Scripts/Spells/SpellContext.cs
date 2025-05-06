using UnityEngine;
using System.Collections.Generic;

public class SpellContext
{
    public float Power;
    public int Wave;

    public Spell.Modifiers Mods;

    public Dictionary<string, float> ToDictionary()
    {
        return new Dictionary<string, float>
        {
            { "power", Power },
            { "wave", Wave }
        };
    }
}
