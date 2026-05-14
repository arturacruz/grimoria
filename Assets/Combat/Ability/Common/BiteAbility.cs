using UnityEngine;

public class BiteAbility : Ability
{
    public override string description => $"Deal {damage} damage.";
    public override uint damage => 10;

    public BiteAbility(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies)
    {
    }

    public override void DoAbility(Board allies, Board enemies)
    {
        var target = BattleManager.Instance.GetTarget(owner);
        if (target == null)
        {
            Debug.Log($"{owner.name} found no target.");
            return;
        }
        BattleManager.Instance.SpawnAttack(owner, target, damage);
    }
}