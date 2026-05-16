public sealed class Blood : Element
{

    public override string elementName => "Blood";
    public override string countersName => "Void";
    public override string description => $"Heals equal to {value:P}% of damage. Counters {countersName}.";
    protected override float value => 0.25f;

    public Blood(Creature creature)
    {
        owner = creature;
    }

    public override void DoOnStart(Creature[] targets, uint damage)
    {
    }

    public override void DoAbility(Creature[] targets, uint damage)
    {
        var amount = 0f;
        foreach (var c in targets)
            if (IsCounter(c)) amount += value * 2;
            else amount += value;

        var final = amount * damage; 
        owner.Heal((uint) final);
    }
}
