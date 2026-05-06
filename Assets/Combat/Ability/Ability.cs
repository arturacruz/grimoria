using UnityEngine;

public abstract class Ability : IBattleBehaviour
{
    // TODO: Rethink descriptions to support different values of damage.
    public abstract string description { get; }
    public abstract float levelToValueRatio { get; }
    public abstract void DoOnStart(Board allies, Board enemies);
    public abstract void DoAbility(Board allies, Board enemies);
}