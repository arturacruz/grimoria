using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cerberus : Creature
{
    public override string name => "Cerberus";
    public override Rarity rarity => Rarity.Epic;
    public override Tag tag => Tag.Damned;
    public override BattleClass battleClass => BattleClass.AOE;
    public override byte height => 2;
    public override byte width => 2;
    public override HealthComponent health => _health;
    public override Cooldown cooldown => _cooldown;
    public override string description => "A three-headed beast that guards the gates of fire.";
    public override Element element => _element;
    public override List<Ability> abilities => _abilities;

    private Cooldown _cooldown;
    private HealthComponent _health;
    private Element _element;
    private readonly List<Ability> _abilities = new();

    private void Awake()
    {
        _cooldown = new Cooldown(4f);
        _health = new HealthComponent(250);
        _element = new Blaze(this);
        _abilities.Add(new Gatefire(this));
        _abilities.Add(new BurningGuard(this));
    }
}