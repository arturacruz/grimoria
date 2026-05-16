using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayComponent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] backgrounds;
    [SerializeField] private Image[] images;
    [SerializeField] private TextMeshProUGUI creatureName;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI skills;
    [SerializeField] private TextMeshProUGUI cooldown;
    [SerializeField] private TextMeshProUGUI element;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI rarity;
    [SerializeField] private TextMeshProUGUI creatureTag;
    [SerializeField] private TextMeshProUGUI[] statusEffects;

    private void SetVisibility(bool visible)
    {
        var alpha = visible ? 1 : 0;
        foreach (var background in backgrounds)
            background.enabled = visible;
        foreach (var image in images)
            image.enabled = visible;
        creatureName.alpha = alpha;
        element.alpha = alpha;
        health.alpha = alpha;
        skills.alpha = alpha;
        cooldown.alpha = alpha;
        description.alpha = alpha;
        rarity.alpha = alpha;
        creatureTag.alpha = alpha;
        foreach (var effect in statusEffects)
            effect.alpha = alpha;
    }

    private Vector3 GetPosition(Creature creature)
    {
        var offset = creature.playerSide ? new Vector2(6, 0) : new Vector2(-5, 0);
        return creature.gameObject.transform.position + (Vector3) offset;
    }

    private void FixedUpdate()
    {
        var creature = GameManager.Instance.HoveringCreature;
        if (creature == null || GameManager.Instance.SelectedCreature == creature)
        {
            SetVisibility(false);
            return;
        }

        var c = creature.GetComponent<Creature>();
        SetVisibility(true);
        transform.position = GetPosition(c);
        creatureName.text = c.name;
        health.text = $"{c.health.health}\n{c.health.maxHealth}";
        element.text = $"{c.element.elementName}: {c.element.description}";
        skills.text = "";
        foreach (var ab in c.abilities)
            skills.text += "- " + ab.description + "\n";
        cooldown.text = $"{c.cooldown.timeSeconds:F1}s";
        description.text = c.description;
        rarity.text = $"Rarity: {c.GetRarityAsString()}";
        creatureTag.text = $"Tag: {c.GetTagAsString()}";

        for (var i = 0; i < statusEffects.Length; i++)
        {
            statusEffects[i].text = $"{c.GetStatusAmount((Status.StatusEffect)i)}";
        }
    }
}
