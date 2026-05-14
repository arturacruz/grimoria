using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public readonly Queue<Creature> DeathPool = new();

    [SerializeField] private Board player;
    [SerializeField] private Board enemy;
    [SerializeField] private GameObject trailAttackPrefab;

    public bool battleOngoing;
    
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
        player.StartBattle();
        enemy.StartBattle();
        
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
        
        HandleDeaths();
    }
    
    public Creature GetTarget(Creature attacker)
    {
        var isPlayerAttacking = attacker.playerSide;
        var attackingBoard = isPlayerAttacking ? player : enemy;
        var targetBoard = isPlayerAttacking ? enemy : player;

        var y = attackingBoard.GetPositionOfCreature(attacker, true).y;

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
    
    private void HandleDeaths()
    {
        while (DeathPool.Count > 0)
        {
            var c = DeathPool.Dequeue();
            KillCreature(c);
        }
    }

    public void SpawnAttack(Creature from, Creature to, uint damage)
    {
        var fromc = from.gameObject;
        var obj = Instantiate(trailAttackPrefab, fromc.transform.position, fromc.transform.rotation);
        var trail = obj.GetComponent<TrailComponent>();
        trail.damage = damage;
        trail.target = to;
    }
    

    public void UnlogCreature(Creature creature)
    {
        DeathPool.Enqueue(creature);
    }

    private void KillCreature(Creature creature)
    {
        var end = creature.playerSide ? player.DestroyCreature(creature) : enemy.DestroyCreature(creature);

        if (!end)
            return;
        
        battleOngoing = false;
        Debug.Log("battle ended");
    }
}