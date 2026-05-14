using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour, IBattleBehaviour
{
    public abstract string name { get; }
    public abstract BattleClass battleClass { get; } 
    public abstract byte height { get; }
    public abstract byte width { get;  }
    public abstract HealthComponent health { get; }
    public abstract Cooldown cooldown { get; }
    public abstract string description { get; }
    protected abstract Element element { get; }
    protected abstract List<Ability> abilities { get; }
    [SerializeField] private LifeBarComponent lifeBar;
    public bool playerSide = true;
    public int level = 1;

    private void Start()
    {
        lifeBar.Init(health.maxHealth);
    }

    public void DoOnStart(Board allies, Board enemies)
    {
        element.DoOnStart(allies, enemies);
        foreach (var ability in abilities)
            ability?.DoOnStart(allies, enemies);
        
        cooldown.Restart();
    }

    public void DoAbility(Board allies, Board enemies)
    {
        if (!cooldown.IsDone())
            return;
        
        element.DoAbility(allies, enemies);
        foreach (var ability in abilities)
            ability?.DoAbility(allies, enemies);

        cooldown.Restart();
    }

    public void TakeDamage(uint damage)
    {
        if (health.TakeDamage(damage))
            BattleManager.Instance.CreatureDied.Invoke(this);

        lifeBar.UpdateValue(health.health);
    }
}

public enum BattleClass
{
    Meele, Flank, AOE
}
