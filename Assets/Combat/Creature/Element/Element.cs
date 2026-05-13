using UnityEngine;

public abstract class Element : IBattleBehaviour
{
    public string elementName;

    public abstract void DoOnStart(Board allies, Board enemies);
    public abstract void DoAbility(Board allies, Board enemies);
}
