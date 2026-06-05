using UnityEngine;

public class RavenousClaw : Ability
{
    public override string abilityName => "Ravenous Claw";
    public override string description => $"Deals {GetDamageDescriptionValue} damage. Gains that amount as damage.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 3;

    public RavenousClaw(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies) {}

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        var target = BattleManager.Instance.GetTarget(owner);
        if (target.Length == 0 || target[0] == null)
            return target;
        BattleManager.Instance.SpawnAttack(owner, target[0], currentDamage);
        bonusDamage += currentDamage;
        return target;
    }
}
