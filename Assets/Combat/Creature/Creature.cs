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
    public int level = 1;

    public void DoOnStart(Board allies, Board enemies)
    {
        element.DoOnStart(allies, enemies);
        foreach (var ability in abilities)
            ability.DoOnStart(allies, enemies);
    }

    public void DoAbility(Board allies, Board enemies) 
    {
        element.DoAbility(allies, enemies);
        foreach (var ability in abilities)
            ability.DoAbility(allies, enemies);
    }

    public void TakeDamage(uint damage)
    {
        if (health.TakeDamage(damage))
            BattleManager.Instance.CreatureDied.Invoke(this);
    }
}

public enum BattleClass
{
    Meele, Flank, AOE
}
