using UnityEngine;

public class Gatefire : Ability
{
    public override string abilityName => "Gatefire";
    public override string description => $"Apply {GetDamageDescriptionValue} burn to all enemies.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 5;

    public Gatefire(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies) {}

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        var targets = BattleManager.Instance.GetTarget(owner);
        foreach (var target in targets)
        {
            BattleManager.Instance.SpawnAttack(owner, target, 0);
            target?.ApplyStatus(Status.StatusEffect.Burn, currentDamage);
        }
        return targets;
    }
}