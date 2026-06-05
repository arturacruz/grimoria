using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayComponent : MonoBehaviour
{
    [SerializeField] private Vector2 playerOffset = new(6, 0);
    [SerializeField] private Vector2 enemyOffset = new(-5, 0);
    [SerializeField] private Vector2 screenPadding = new(0.4f, 0.4f);

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
        var offset = creature.playerSide ? playerOffset : enemyOffset;
        var position = creature.gameObject.transform.position + (Vector3) offset;

        var bounds = GetOverlayBounds();
        if (Camera.main == null || !bounds.HasValue)
            return position;

        var camera = Camera.main;
        var camHeight = camera.orthographicSize;
        var camWidth = camHeight * camera.aspect;
        var camPos = camera.transform.position;

        var halfSize = bounds.Value.extents;
        var minX = camPos.x - camWidth + halfSize.x + screenPadding.x;
        var maxX = camPos.x + camWidth - halfSize.x - screenPadding.x;
        var minY = camPos.y - camHeight + halfSize.y + screenPadding.y;
        var maxY = camPos.y + camHeight - halfSize.y - screenPadding.y;

        if (position.x + halfSize.x > camPos.x + camWidth - screenPadding.x)
            position.x = creature.gameObject.transform.position.x - Mathf.Abs(offset.x);
        else if (position.x - halfSize.x < camPos.x - camWidth + screenPadding.x)
            position.x = creature.gameObject.transform.position.x + Mathf.Abs(offset.x);

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        return position;
    }

    private Bounds? GetOverlayBounds()
    {
        var hasBounds = false;
        var bounds = new Bounds(transform.position, Vector3.zero);

        foreach (var background in backgrounds)
        {
            if (background == null)
                continue;

            if (!hasBounds)
            {
                bounds = background.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(background.bounds);
            }
        }

        return hasBounds ? bounds : null;
    }

    private void FixedUpdate()
    {
        var creature = GameManager.Instance.LockedDescriptionCreature != null
            ? GameManager.Instance.LockedDescriptionCreature
            : GameManager.Instance.HoveringCreature;

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
        cooldown.text = $"{c.cooldown.timeSeconds:F}s";
        description.text = c.description;
        rarity.text = $"Rarity: {c.GetRarityAsString()}";
        creatureTag.text = $"Tag: {c.GetTagAsString()}";

        for (var i = 0; i < statusEffects.Length; i++)
        {
            statusEffects[i].text = $"{c.GetStatusAmount((Status.StatusEffect)i)}";
        }
    }
}
