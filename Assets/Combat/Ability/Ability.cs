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

    protected uint DamageValue
    {
        get
        {
            if (owner.GetStatusAmount(Status.StatusEffect.Weak) > 0)
                return (uint)((damage + bonusDamage) * 0.8f);
            return damage + bonusDamage;
        }
    }

    protected uint GetDamageDescriptionValue
    {
        get => DamageValue;
    }

    public void DoAbility(Board allies, Board enemies)
    {
        currentDamage = DamageValue;
        var targets = DoOnActivate(allies, enemies);
        if (targets == null)
            targets = new Creature[] { };
        owner.element.DoAbility(targets, currentDamage);
    }

    public void OnSkillUsed(Creature from, Creature to)
    {
        currentDamage = DamageValue;
        var targets = OnSkill(from, to);
        if (targets == null)
            targets = new Creature[] { };
        owner.element.DoAbility(targets, currentDamage);
    }
    
    public virtual void OnBurnApplied() {}

    public virtual Creature[] OnSkill(Creature from, Creature to)
    {
        return new Creature[] { };
    }

    protected abstract Creature[] DoOnActivate(Board allies, Board enemies);
}
