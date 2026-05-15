using UnityEngine;

public class BiteAbility : Ability
{
    public override string description => $"Deal {GetDamageDescriptionValue} damage.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 10;

    public BiteAbility(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies) {}

    protected override void DoOnActivate(Board allies, Board enemies)
    {
        var target = BattleManager.Instance.GetTarget(owner);
        if (target == null)
        {
            Debug.Log($"{owner.name} found no target.");
            return;
        }
        BattleManager.Instance.SpawnAttack(owner, target, currentDamage);
    }
}