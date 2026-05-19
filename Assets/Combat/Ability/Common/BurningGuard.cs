using UnityEngine;

public class BurningGuard: Ability
{
    public override string abilityName => "BurningGuard";
    public override string description => $"Whenever burn is applied, gain and heal {GetDamageDescriptionValue} max HP.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 3;

    public BurningGuard(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies) {}

    public override void OnBurnApplied()
    {
        owner.health.maxHealth += currentDamage;
        owner.health.health += (int) currentDamage;
    }

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        return new Creature[] {};
    }
}