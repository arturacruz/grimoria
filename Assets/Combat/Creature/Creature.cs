using System;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity
{
    Common, Rare, Epic
}

public enum Tag
{
    Beast, Undead, Damned
}

public abstract class Creature : MonoBehaviour, IBattleBehaviour
{
    public string GetRarityAsString() => rarity switch
    { 
        Rarity.Common => "Common",
        Rarity.Rare => "Rare",
        Rarity.Epic => "Epic",
        _ => "Error"
    };
    public string GetTagAsString() => tag switch
    { 
        Tag.Beast => "Beast",
        Tag.Damned => "Damned",
        Tag.Undead => "Undead",
        _ => "Error"
    };
    
    public abstract string name { get; }
    public abstract Rarity rarity { get; }
    public abstract Tag tag { get; }
    public abstract BattleClass battleClass { get; } 
    public abstract byte height { get; }
    public abstract byte width { get;  }
    public abstract HealthComponent health { get; }
    public abstract Cooldown cooldown { get; }
    public abstract string description { get; }
    public abstract Element element { get; }
    public abstract List<Ability> abilities { get; }
    public bool playerSide = true;
    public uint[] statusEffects = new uint[Status.Amount];
    private bool dead;

    public void DoOnStart(Board allies, Board enemies)
    {
        cooldown.started = true;
        element.DoOnStart(allies, enemies);
        foreach (var ability in abilities)
            ability?.DoOnStart(allies, enemies);
        
        cooldown.Restart();
    }

    public uint GetStatusAmount(Status.StatusEffect effect)
    {
        return statusEffects[(int)effect];
    }

    private void LowerStatusAmount(Status.StatusEffect effect)
    {
        statusEffects[(int)effect]--;
    }

    public void ApplyStatus(Status.StatusEffect effect, uint value)
    {
        OnApplyStatus(effect, value);
    }

    public void DoAbility(Board allies, Board enemies)
    {
        if (!cooldown.IsDone())
            return;
        
        element.DoAbility(allies, enemies);
        foreach (var ability in abilities)
            ability?.DoAbility(allies, enemies);
        
        if (GetStatusAmount(Status.StatusEffect.Weak) > 0)
            LowerStatusAmount(Status.StatusEffect.Weak);

        cooldown.Restart();
    }

    private void Die()
    {
        if (dead)
            return;
        BattleManager.Instance.UnlogCreature(this);
        dead = true;
    }

    public void TakeDamage(uint damage)
    {
        if (dead)
            return;
        if (health.TakeDamage(damage))
            Die();
    }

    public void DoOnTick()
    {
        if (GetStatusAmount(Status.StatusEffect.Burn) is uint value and > 0)
        {
            TakeDamage(value);
            LowerStatusAmount(Status.StatusEffect.Burn);
        }
    }

    private void OnApplyStatus(Status.StatusEffect effect, uint value)
    {
        statusEffects[(int)effect] += value;
        switch (effect)
        {
            case Status.StatusEffect.Ruin:
                if (health.health <= GetStatusAmount(Status.StatusEffect.Ruin))
                    Die();
                break;
        }
    }
}

public enum BattleClass
{
    Meele, Flank, AOE
}
