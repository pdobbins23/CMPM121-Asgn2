using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellRewardUI : MonoBehaviour
{
    public static SpellRewardUI Instance;

    public GameObject panel;
    public TextMeshProUGUI spellNameText;
    public TextMeshProUGUI spellDescText;
    public Image iconImage;

    public Button[] replaceButtons;
    public Button declineButton;

    private Spell rewardSpell;
    private SpellCaster player;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(Spell newSpell, SpellCaster caster)
    {
        rewardSpell = newSpell;
        player = caster;

        spellNameText.text = rewardSpell.GetName();
        spellDescText.text = $"Mana: {rewardSpell.GetManaCost()}\n" +
                             $"Damage: {rewardSpell.GetBaseDamage()}\n" +
                             $"Cooldown: {rewardSpell.GetCoolDown()}";

        GameManager.Instance.spellIconManager.PlaceSprite(rewardSpell.GetIcon(), iconImage);

        panel.SetActive(true);
    }

    public void ReplaceSpell(int index)
    {
        player.spells[index] = rewardSpell;
        Close();
    }

    public void Decline()
    {
        Close();
    }

    private void Close()
    {
        panel.SetActive(false);
    }
}
