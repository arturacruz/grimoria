using System;
using TMPro;
using UnityEngine;

public class OverlayComponent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer combatClass;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI cooldown;

    private void SetVisibility(bool visible)
    {
        var alpha = visible ? 1 : 0;
        background.enabled = visible;
        combatClass.enabled = visible;
        name.alpha = alpha;
        health.alpha = alpha;
        damage.alpha = alpha;
        cooldown.alpha = alpha;
    }

    private Vector3 GetPosition(Creature creature)
    {
        var offset = creature.playerSide ? new Vector2(5, 0) : new Vector2(-5, 0);
        return creature.gameObject.transform.position + (Vector3) offset;
    }
    
    private void FixedUpdate()
    {
        var creature = GameManager.Instance.HoveringCreature;
        SetVisibility(creature != null);
        if (creature == null)
            return;

        var c = creature.GetComponent<Creature>();
        transform.position = GetPosition(c);
        name.text = c.name;
        health.text = $"HP: {c.health.health}/{c.health.maxHealth}";
        damage.text = c.abilities[0].description;
        cooldown.text = $"Cooldown: {(int) c.cooldown.ElapsedTimeSec()}/{(int) c.cooldown.timeSeconds}";
    }
}
