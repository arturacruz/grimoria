using UnityEngine;

public class DeathSentence: Ability
{
    public override string abilityName => "DeathSentence";
    public override string description => $"Applies {damage} ruin.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 10;

    public DeathSentence(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies)
    {
        
    }

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        var target = BattleManager.Instance.GetTarget(owner);
        target[0].ApplyStatus(Status.StatusEffect.Ruin, currentDamage);
        return target;
    }
}