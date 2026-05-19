using System.Collections.Generic;

public sealed class Imp: Creature
{
    public override string name => "Imp";
    public override Rarity rarity => Rarity.Common;
    public override Tag tag => Tag.Damned;
    public override BattleClass battleClass => BattleClass.Flank;
    public override byte height => 1;
    public override byte width => 1;
    public override HealthComponent health => _health;
    public override Cooldown cooldown => _cooldown;
    public override string description => "A small, fragile and cruel demon. Destructive against backlines.";
    public override Element element => _element;
    public override List<Ability> abilities => _abilities;

    private Cooldown _cooldown;
    private HealthComponent _health;
    private Element _element;
    private readonly List<Ability> _abilities = new();

    private void Awake()
    {
        _cooldown = new Cooldown(2f);
        _health = new HealthComponent(30);
        _element = new Blaze(this);
        _abilities.Add(new WickedShot(this));
    }
}