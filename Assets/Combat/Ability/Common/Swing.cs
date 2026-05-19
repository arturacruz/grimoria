using UnityEngine;

public class Swing: Ability
{
    public override string abilityName => "Swing";
    public override string description => $"Deal {GetDamageDescriptionValue} damage ({1/10f:P} of max HP). Gains HP equal to damage dealt.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => owner.health.maxHealth / 10;

    public Swing(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies) {}

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        var target = BattleManager.Instance.GetTarget(owner);
        BattleManager.Instance.SpawnAttack(owner, target[0], currentDamage);
        owner.health.maxHealth += currentDamage;
        owner.health.health += (int)currentDamage;
        return target;
    }
}