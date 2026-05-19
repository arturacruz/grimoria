using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpiderQueen : Creature
{
    public override string name => "Spider Queen";
    public override Rarity rarity => Rarity.Rare;
    public override Tag tag => Tag.Beast;
    public override BattleClass battleClass => BattleClass.Meele;
    public override byte height => 2;
    public override byte width => 2;
    public override HealthComponent health => _health;
    public override Cooldown cooldown => _cooldown;
    public override string description => "Quick and lethal, leader and stronger with others of it's kind.";
    public override Element element => _element;
    public override List<Ability> abilities => _abilities;

    private Cooldown _cooldown;
    private HealthComponent _health;
    private Element _element;
    private readonly List<Ability> _abilities = new();

    private void Awake()
    {
        _cooldown = new Cooldown(5f);
        _health = new HealthComponent(150);
        _element = new Shadow(this);
        _abilities.Add(new QueenOfBeasts(this));
        _abilities.Add(new SpiderBrood(this));
    }
}