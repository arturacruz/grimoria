public abstract class Element
{
    protected Creature owner;
    public abstract string elementName { get; }
    public abstract string countersName { get; }
    public abstract string description { get; }
    protected abstract float value { get; }
    
    protected bool IsCounter(Creature creature)
    {
        return creature.element.elementName == countersName;
    }

    public abstract void DoOnStart(Creature[] enemies, uint damage);
    public abstract void DoAbility(Creature[] enemies, uint damage);
}
