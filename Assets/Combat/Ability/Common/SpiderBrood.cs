using UnityEngine;

public class SpiderBrood: Ability
{
    public override string abilityName => "One Woman Army";
    public override string description => $"At the start of combat, all spiders have -0.5s cooldown.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 0;

    public SpiderBrood(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies)
    {
        foreach (var creature in allies.GetGrid())
        {
            if (creature == null) continue;
            if (creature.name == "Spider")
                creature.cooldown.timeSeconds -= 0.5f;
        }
    }

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        return new Creature[] { };
    }
}