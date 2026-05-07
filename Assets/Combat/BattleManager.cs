using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public readonly UnityEvent<Creature> CooldownEnded;
    public readonly UnityEvent<Creature> CreatureDied = new();

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

    private void Start()
    {
        Instance.CreatureDied.AddListener(OnCreatureDeath);
    }

    private void FixedUpdate()
    {
        if (battleOngoing)
            BattleLoop();
    }

    public void StartBattle()
    {
        Debug.Log("battle start");
        foreach (var creature in player.GetGrid())
            creature?.DoOnStart(player, enemy);
        
        foreach (var creature in enemy.GetGrid())
            creature?.DoOnStart(enemy, player);

        battleOngoing = true;
    }

    private void BattleLoop()
    {
        foreach (var creature in player.GetGrid())
            creature?.DoAbility(player, enemy);
        
        foreach (var creature in enemy.GetGrid())
            creature?.DoAbility(player, enemy);
    }
    
    public Creature GetTarget(Creature attacker)
    {
        var isPlayerAttacking = player.ContainsCreature(attacker);
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

    private void OnCreatureDeath(Creature creature)
    {
        Destroy(creature.gameObject);
        battleOngoing = false;
        Debug.Log("battle ended");
    }
}