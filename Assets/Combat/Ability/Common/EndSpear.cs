using UnityEngine;

public class EndSpear : Ability
{
    public override string abilityName => "End Spear";
    public override string description => $"Deals {damage} damage and applies 1 ruin to all enemies.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 6;

    public EndSpear(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies)
    {
    }

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        var targets = BattleManager.Instance.GetTarget(owner);
        foreach (var target in targets)
        {
            if (target == null) continue;
            BattleManager.Instance.SpawnAttack(owner, target, currentDamage);
            target.ApplyStatus(Status.StatusEffect.Ruin, 1);
        }
        return targets;
    }
}