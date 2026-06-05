using UnityEngine;

public class QueenOfBeasts: Ability
{
    public override string abilityName => "Queen of Beasts";
    public override string description => $"All beast creatures except this gain +{damage}.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 3;

    public QueenOfBeasts(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies)
    {
    }

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        foreach (var ally in allies.GetGrid())
        {
            if (ally == null || ally == owner || ally.tag != Tag.Beast) continue;
            foreach (var ability in ally.abilities)
                ability.bonusDamage += DamageValue;
        }
        return new Creature[] { };
    }
}
