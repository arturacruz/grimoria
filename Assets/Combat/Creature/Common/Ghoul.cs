using System.Collections.Generic;

public class Ghoul : Creature
{
    public override string name => "Ghoul";
    public override byte height => 2;
    public override byte width => 1;
    public override HealthComponent health => new HealthComponent(150);
    public override float cooldown => 5f;
    public override string description => "Just a ghoul.";
    protected override Element element => new Blood();
    protected override List<Ability> abilities => new();

    private void Start()
    {
        abilities.Add(new BiteAbility());
    }
}