using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class SpellManager
{
    public Dictionary<string, JObject> AllSpells = new Dictionary<string, JObject>();

    private static SpellManager theInstance;
    public static SpellManager Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new SpellManager();
            return theInstance;
        }
    }

    private SpellManager()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("spells");
        JToken jo = JToken.Parse(jsonFile.text);

        foreach (var spell in jo.Children<JProperty>())
        {
            AllSpells[spell.Name] = (JObject)spell.Value;
        }
    }
}
