using UnityEngine;

public class OneWomanArmy : Ability
{
    public override string abilityName => "One Woman Army";
    public override string description => $"At the start of combat, gains +{damage} hp for each creature in combat.";
    // DO NOT USE DAMAGE FOR ACTUAL DAMAGE. Use currentDamage
    public override uint damage => 5;

    public OneWomanArmy(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Board allies, Board enemies)
    {
        uint amount = 0;
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (allies.GetGrid()[i, j] != null)
                    amount++;
                if (enemies.GetGrid()[i, j] != null)
                    amount++;
            }
        }

        amount *= DamageValue;
        owner.health.maxHealth += amount;
        owner.health.health += (int) amount;
        
        Debug.Log($"hp gain: {amount}");
    }

    protected override Creature[] DoOnActivate(Board allies, Board enemies)
    {
        return new Creature[] { };
    }
}
