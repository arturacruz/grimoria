using UnityEngine;

public class WickedShot: Ability
{
    public override string abilityName => "Wicked Shot";
    public override string description => $"Deal {GetDamageDescriptionValue} damage. Loses {GetDamageDescriptionValue / 2} HP.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 20;

    public WickedShot(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies) {}

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        var target = BattleManager.Instance.GetTarget(owner);
        BattleManager.Instance.SpawnAttack(owner, target[0], currentDamage);
        owner.TakeDamage(currentDamage / 2);
        return target;
    }
}