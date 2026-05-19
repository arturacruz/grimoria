using UnityEngine;

public abstract class Ability : IBattleBehaviour
{
    protected Creature owner;
    // TODO: Rethink descriptions to support different values of damage.
    public abstract string abilityName { get; }
    public abstract string description { get; }
    public abstract uint damage { get; }
    public uint currentDamage;
    public uint bonusDamage;
    public abstract void DoOnStart(Board allies, Board enemies);

    protected uint GetDamageDescriptionValue
    {
        get
        {
            if (owner.GetStatusAmount(Status.StatusEffect.Weak) > 0)
                return (uint) ((damage + bonusDamage) * 0.8);
            return damage + bonusDamage;
        }
    }

    public void DoAbility(Board allies, Board enemies)
    {
        if (owner.GetStatusAmount(Status.StatusEffect.Weak) > 0)
            currentDamage = (uint)((damage + bonusDamage) * 0.8);
        else
            currentDamage = damage + bonusDamage;
        var targets = DoOnActivate(allies, enemies);
        owner.element.DoAbility(targets, currentDamage);
    }

    public void OnSkillUsed(Creature from, Creature to)
    {
        var targets = OnSkill(from, to);
        owner.element.DoAbility(targets, currentDamage);
    }
    
    public virtual void OnBurnApplied() {}

    public virtual Creature[] OnSkill(Creature from, Creature to)
    {
        return new Creature[] { };
    }

    protected abstract Creature[] DoOnActivate(Board allies, Board enemies);
}