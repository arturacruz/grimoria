using System.Collections.Generic;

public sealed class Spider : Creature
{
    public override string name => "Spider";
    public override BattleClass battleClass => BattleClass.Meele;
    public override byte height => 1;
    public override byte width => 1;
    public override HealthComponent health => new(100);
    public override Cooldown cooldown => new(3f);
    public override string description => "Just a spider.";
    protected override Element element => new Plague();
    protected override List<Ability> abilities => new();

    private void Start()
    {
        abilities.Add(new BiteAbility(this));
    }
}