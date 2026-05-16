public sealed class Void : Element
{

    public override string elementName => "Void";
    public override string countersName => "Shadow";
    public override string description => $"Increases the enemy's cooldown by {value:P}. Counters {countersName}.";
    protected override float value => 0.01f;
    
    public Void(Creature creature)
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
            if (c == null) continue;
            var amount = value;
            if (IsCounter(c)) 
                amount *= 2f;
            c.SetCooldownByRatio(1 + amount);
        }
    }
}
