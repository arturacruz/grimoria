using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ghoul : Creature
{
    public override string name => "Ghoul";
    public override Rarity rarity => Rarity.Common;
    public override Tag tag => Tag.Damned;
    public override BattleClass battleClass => BattleClass.Meele;
    public override byte height => 2;
    public override byte width => 1;
    public override HealthComponent health => _health;
    public override Cooldown cooldown => _cooldown;
    public override string description => "Just a ghoul.";
    public override Element element => _element;
    public override List<Ability> abilities => _abilities;

    private Cooldown _cooldown;
    private HealthComponent _health;
    private Element _element;
    private readonly List<Ability> _abilities = new();

    private void Awake()
    {
        _cooldown = new Cooldown(6f);
        _health = new HealthComponent(100);
        _element = new Blood();
        _abilities.Add(new BiteAbility(this));
    }
}