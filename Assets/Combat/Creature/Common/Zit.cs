using System.Collections.Generic;

public class Zit : Creature
{
    public override string name => "Zit";
    public override Rarity rarity => Rarity.Legendary;
    public override Tag tag => Tag.Damned;
    public override BattleClass battleClass => BattleClass.AOE;
    public override byte height => 2;
    public override byte width => 2;
    public override HealthComponent health => _health;
    public override Cooldown cooldown => _cooldown;
    public override string description => "Master of the underworld.";
    public override Element element => _element;
    public override List<Ability> abilities => _abilities;

    private Cooldown _cooldown;
    private HealthComponent _health;
    private Element _element;
    private readonly List<Ability> _abilities = new();

    private void Awake()
    {
        _cooldown = new Cooldown(3f);
        _health = new HealthComponent(250);
        _element = new Death(this);
        _abilities.Add(new EndSpear(this));
        _abilities.Add(new NullRay(this));
    }
}
