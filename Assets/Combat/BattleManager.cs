using System;
using UnityEngine;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public readonly UnityEvent<Creature> CooldownEnded;
    public readonly UnityEvent<Creature> CreatureDied;

    [SerializeField] private Board player;
    [SerializeField] private Board enemy;

    private bool battleOngoing;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void FixedUpdate()
    {
        if (battleOngoing)
            BattleLoop();
    }

    public void StartBattle()
    {
        foreach (var creature in player.grid)
            creature.DoOnStart(player, enemy);
        
        foreach (var creature in enemy.grid)
            creature.DoOnStart(enemy, player);

        battleOngoing = true;
    }

    private void BattleLoop()
    {
        foreach (var creature in player.grid)
            creature.DoAbility(player, enemy);
        
        foreach (var creature in enemy.grid)
            creature.DoAbility(enemy, player);
    }
    
    public Creature GetTarget(Creature attacker, bool isPlayerAttacking)
    {
        var attackingBoard = isPlayerAttacking ? player : enemy;
        var targetBoard = isPlayerAttacking ? enemy : player;

        var y = attackingBoard.GetPositionOfCreature(attacker).y;

        switch (attacker.battleClass)
        {
            case BattleClass.Meele:
                return targetBoard.GetMeleeTargetAt(y);
            case BattleClass.Flank: // TODO
                return null;
            case BattleClass.AOE: // TODO
                return null;
        }

        return null;
    }
}