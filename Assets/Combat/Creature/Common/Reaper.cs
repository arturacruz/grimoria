using System.Collections.Generic;

public sealed class Reaper: Creature
{
    public override string name => "Reaper";
    public override Rarity rarity => Rarity.Epic;
    public override Tag tag => Tag.Undead;
    public override BattleClass battleClass => BattleClass.Flank;
    public override byte height => 2;
    public override byte width => 1;
    public override HealthComponent health => _health;
    public override Cooldown cooldown => _cooldown;
    public override string description => "The personification of death itself.";
    public override Element element => _element;
    public override List<Ability> abilities => _abilities;

    private Cooldown _cooldown;
    private HealthComponent _health;
    private Element _element;
    private readonly List<Ability> _abilities = new();

    private void Awake()
    {
        _cooldown = new Cooldown(6f);
        _health = new HealthComponent(200);
        _element = new Death(this);
        _abilities.Add(new Harvest(this));
        _abilities.Add(new DeathSentence(this));
    }
}