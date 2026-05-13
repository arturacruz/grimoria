using System.Collections.Generic;

public sealed class Spider : Creature
{
    public override string name => "Spider";
    public override BattleClass battleClass => BattleClass.Meele;
    public override byte height => 1;
    public override byte width => 1;
    public override HealthComponent health => _health;
    public override Cooldown cooldown => _cooldown;
    public override string description => "Just a spider.";
    protected override Element element => _element;
    protected override List<Ability> abilities => _abilities;

    private Cooldown _cooldown;
    private HealthComponent _health;
    private Element _element;
    private readonly List<Ability> _abilities = new();

    private void Awake()
    {
        _cooldown = new Cooldown(3f);
        _health = new HealthComponent(100);
        _element = new Plague();
        _abilities.Add(new BiteAbility(this));
    }
}