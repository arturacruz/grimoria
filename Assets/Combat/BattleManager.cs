using System.Collections.Generic;
using Combat.BoardPreset;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

enum BattleResult
{
    None,
    Win,
    Defeat
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AudioClip attackClip;
    public readonly Queue<Creature> DeathPool = new();
    public readonly UnityEvent ApplyBurn = new();

    [SerializeField] public Board player;
    [SerializeField] private Board enemy;
    [SerializeField] private GameObject trailAttackPrefab;
    [SerializeField] private GameObject nextSceneButton;
    [SerializeField] public bool isBoss;
    private Cooldown tickCooldown;
    
    public bool battleOngoing;
    private BattleResult result = BattleResult.None;

    public bool IsCreatureInPlayerGrid(Creature creature)
    {
        return player.ContainsCreature(creature);
    }
    
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        Instance = this;
        nextSceneButton.SetActive(false);
        tickCooldown = new Cooldown(0.5f);
        ApplyBurn.AddListener(OnApplyBurn);
    }

    private void OnApplyBurn()
    {
        foreach (var c in player.GetGrid())
        {
            if (c == null) continue;
            foreach (var ab in c.abilities)
                ab.OnBurnApplied();
        }
        
        foreach (var c in enemy.GetGrid())
        {
            if (c == null) continue;
            foreach (var ab in c.abilities)
                ab.OnBurnApplied();
        }
    }

    private void FixedUpdate()
    {
        if (battleOngoing)
            BattleLoop();
    }

    public void StartBattle()
    {
        if (battleOngoing) return;
        Debug.Log("start battle");
        player.StartBattle();
        enemy.StartBattle();
        tickCooldown.Start();
        
        //foreach (var creature in enemy.GetGrid())
          //  creature?.ApplyStatus(Status.StatusEffect.Burn, 100);
        
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

        // Tick logic
        if (tickCooldown.IsDone())
        {
            foreach (var creature in player.GetGrid())
                creature?.DoOnTick();

            foreach (var creature in enemy.GetGrid())
                creature?.DoOnTick();
            
            tickCooldown.Restart();
        }
        
        HandleDeaths();
    }
    
    public Creature[] GetTarget(Creature attacker)
    {
        var isPlayerAttacking = attacker.playerSide;
        var attackingBoard = isPlayerAttacking ? player : enemy;
        var targetBoard = isPlayerAttacking ? enemy : player;

        var y = attackingBoard.GetPositionOfCreature(attacker, true).y;

        switch (attacker.battleClass)
        {
            case BattleClass.Meele:
                return new[] {targetBoard.GetMeleeTargetAt(y)};
            case BattleClass.Flank: // TODO
               return new[] {targetBoard.GetFlankTargetAt(y)};
            case BattleClass.AOE: // TODO
                return targetBoard.GetAOETargets();
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

        trail.element = from.element;
        
        if (attackAudioSource != null && attackClip != null)
            attackAudioSource.PlayOneShot(attackClip);

        foreach (var creature in player.GetGrid())
        {
            if (creature == null) continue;
            foreach (var ability in creature.abilities)
                ability.OnSkillUsed(from, to);
        }

        foreach (var creature in enemy.GetGrid())
        {
            if (creature == null) continue;
            foreach (var ability in creature.abilities)
                ability.OnSkillUsed(from, to);
        }
    }
    

    public void UnlogCreature(Creature creature)
    {
        DeathPool.Enqueue(creature);
    }

    private void KillCreature(Creature creature)
    {
        if (creature.playerSide && player.DestroyCreature(creature))
            result = BattleResult.Defeat;
        else if (!creature.playerSide && enemy.DestroyCreature(creature))
            result = BattleResult.Win;

        if (result == BattleResult.None)
            return;
        
        battleOngoing = false;
        
        //novo -mudar caso de problema
        if (result == BattleResult.Win)
        {
            InventoryManager.Instance.AddMoney(50);
        }
        //termina aqui
        
        nextSceneButton.SetActive(true);
        Debug.Log("battle ended");
    }

    public void GoToNextScene()
    {
        GameManager.Instance.battles++;
        if (isBoss)
        {
            if (result == BattleResult.Win)
                SceneManager.LoadScene(3);
            else
                SceneManager.LoadScene(5);
        }
        else
        {
            if (result == BattleResult.Win)
                SceneManager.LoadScene(3);
            else
                SceneManager.LoadScene(0);
        }
    }
}