using UnityEngine;

public class Protector: Ability
{
    public override string abilityName => "Protector";
    public override string description => $"Whenever your team uses a skill, gains {GetDamageDescriptionValue} HP.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 8;

    public Protector(Creature creature)
    {
        owner = creature;
    }
    
    public override Creature[] OnSkill(Creature from, Creature to)
    {
        if (from != null && from.playerSide == owner.playerSide)
        {
            owner.health.maxHealth += DamageValue;
            owner.health.health += (int)DamageValue;
        }

        return new Creature[] { };
    }

    public override void DoOnStart(Board allies, Board enemies) {}

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        var target = BattleManager.Instance.GetTarget(owner);
        if (target.Length == 0 || target[0] == null)
            return target;

        BattleManager.Instance.SpawnAttack(owner, target[0], currentDamage);
        return target;
    }
}
