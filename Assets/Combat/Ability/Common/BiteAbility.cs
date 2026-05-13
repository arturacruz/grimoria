using UnityEngine;

public class BiteAbility : Ability
{
    public override string description => "Deal damage.";
    public override float levelToValueRatio => 1;
    private uint damage = 10;

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
        // TODO: Take level into account
        target.TakeDamage(damage);
        Debug.Log($"bite from {owner.name}!");
    }
}