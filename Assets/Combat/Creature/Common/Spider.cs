using System.Collections.Generic;

public sealed class Spider : Creature
{
    public override string name => "Spider";
    public override byte height => 1;
    public override byte width => 1;
    public override HealthComponent health => new HealthComponent(100);
    public override float cooldown => 3f;
    public override string description => "Just a spider.";
    protected override Element element => new Plague();
    protected override List<Ability> abilities => new();

    private void Start()
    {
        abilities.Add(new BiteAbility());
    }
}