using UnityEngine;

public class OneWomanArmy : Ability
{
    public override string description => $"At the start of combat, gains +{damage} hp for each creature in combat.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 5;

    public OneWomanArmy(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies)
    {
        foreach (var _ in allies.GetGrid())
            owner.health.maxHealth += currentDamage;
        
        foreach (var _ in enemies.GetGrid())
            owner.health.maxHealth += currentDamage;
    }

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        return new Creature[] { };
    }
}