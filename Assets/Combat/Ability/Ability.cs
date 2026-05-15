using UnityEngine;

public abstract class Ability : IBattleBehaviour
{
    protected Creature owner;
    // TODO: Rethink descriptions to support different values of damage.
    public abstract string description { get; }
    public abstract uint damage { get; }
    public uint currentDamage;
    public abstract void DoOnStart(Board allies, Board enemies);

    protected uint GetDamageDescriptionValue
    {
        get
        {
            if (owner.GetStatusAmount(Status.StatusEffect.Weak) > 0)
                return (uint) (damage * 0.8);
            return damage;
        }
    }

    public void DoAbility(Board allies, Board enemies)
    {
        if (owner.GetStatusAmount(Status.StatusEffect.Weak) > 0)
            currentDamage = (uint)(damage * 0.8);
        else
            currentDamage = damage;
        DoOnActivate(allies, enemies);
    }

    protected abstract void DoOnActivate(Board allies, Board enemies);
}