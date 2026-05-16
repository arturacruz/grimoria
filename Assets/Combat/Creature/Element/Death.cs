public sealed class Death : Element
{

    public override string elementName => "Death";
    public override string countersName => "Blaze";
    public override string description => $"Applies {(int) value} ruin. Counters {countersName}.";
    protected override float value => 1f;

    public Death(Creature creature)
    {
        owner = creature;
    }
    
    public override void DoOnStart(Creature[] targets, uint damage)
    {
    }

    public override void DoAbility(Creature[] targets, uint _)
    {
        foreach (var c in targets)
        {
            var val = value;
            if (IsCounter(c))
                val *= 2;
            c.ApplyStatus(Status.StatusEffect.Ruin, (uint) val);
        }
    }
}
