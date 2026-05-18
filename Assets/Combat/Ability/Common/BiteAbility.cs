using UnityEngine;

public class BiteAbility : Ability
{
    public override string abilityName => "Bite";
    public override string description => $"Deal {GetDamageDescriptionValue} damage.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 4;

    public BiteAbility(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies) {}

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        var target = BattleManager.Instance.GetTarget(owner);
        BattleManager.Instance.SpawnAttack(owner, target[0], currentDamage);
        return target;
    }
}