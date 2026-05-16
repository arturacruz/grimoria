public sealed class Shadow : Element
{

    public override string elementName => "Shadow";
    public override string countersName => "Death";
    public override string description => $"Lowers it's cooldown by {value:P}. Counters {countersName}.";
    protected override float value => 0.02f;

    public Shadow(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Creature[] targets, uint damage)
    {
    }

    public override void DoAbility(Creature[] targets, uint _)
    {
        var amount = 0f;
        foreach (var c in targets)
        {
            if (c == null) continue;
            if (IsCounter(c)) amount += value * 2;
            else amount += value;
        }
        owner.SetCooldownByRatio(1 - amount);
    }
}