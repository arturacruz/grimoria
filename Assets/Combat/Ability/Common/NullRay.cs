using UnityEngine;

public class NullRay : Ability
{
    public override string abilityName => "Null Ray";
    public override string description => $"Whenever an enemy uses a skill, deals {damage} damage.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 3;

    public NullRay(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies)
    {
    }

    public override Creature[] OnSkill(Creature from, Creature to)
    {
        if (from.playerSide)
        {
            BattleManager.Instance.SpawnAttack(owner, from, currentDamage);
            return new[] { from };
        }

        return new Creature[] { };
    }


    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        return new Creature[] { };
    }
}