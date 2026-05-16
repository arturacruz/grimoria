public sealed class Plague : Element
{

    public override string elementName => "Plague";
    public override string countersName => "Blood";
    public override string description => $"Applies {(int) value} weak. Counters {countersName}.";
    protected override float value => 1f;
    
    public Plague(Creature creature)
    {
        owner = creature;
    }
    public override void DoOnStart(Creature[] targets, uint damage)
    {
    }

    public override void DoAbility(Creature[] targets, uint damage)
    {
        foreach (var c in targets)
        {
            var val = value;
            if (IsCounter(c))
                val *= 2;
            c.ApplyStatus(Status.StatusEffect.Weak, (uint) val);
        }
    }
}
