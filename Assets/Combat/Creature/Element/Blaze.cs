public sealed class Blaze : Element
{
    public override string elementName => "Blaze";
    public override string countersName => "Plague";
    public override string description => $"Burns enemies equal to {value:P}% of damage. Counters {countersName}.";
    protected override float value => 0.1f;

    public Blaze(Creature creature)
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
            var dmg = damage * value;
            if (IsCounter(c))
                dmg *= 2;
            
            c.ApplyStatus(Status.StatusEffect.Burn, (uint) dmg);
        }
    }
}
