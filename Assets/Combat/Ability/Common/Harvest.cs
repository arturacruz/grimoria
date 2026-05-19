using UnityEngine;

public class Harvest: Ability
{
    public override string abilityName => "Harvest";
    public override string description => $"Deals {damage} for each ruin stack in the target. Heals equal to that amount.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 1;

    public Harvest(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies)
    {
        
    }

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        var target = BattleManager.Instance.GetTarget(owner);
        if (target.Length == 0)
            return target;

        if (target[0] == null)
            return new Creature[] { };
        var amount = currentDamage * target[0].GetStatusAmount(Status.StatusEffect.Ruin);
        BattleManager.Instance.SpawnAttack(owner, target[0], amount);
        owner.Heal(amount);
        return target;
    }
}