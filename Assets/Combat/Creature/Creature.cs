using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Rarity
{
    Common, Rare, Epic, Legendary
}

public enum Tag
{
    Beast, Undead, Damned
}

public abstract class Creature : MonoBehaviour, IBattleBehaviour
{
    public string GetRarityAsString() => rarity switch
    { 
        Rarity.Common => "Common",
        Rarity.Rare => "Rare",
        Rarity.Epic => "Epic",
        Rarity.Legendary => "Legendary",
        _ => "Error"
    };
    public string GetTagAsString() => tag switch
    { 
        Tag.Beast => "Beast",
        Tag.Damned => "Damned",
        Tag.Undead => "Undead",
        _ => "Error"
    };
    
    public abstract string name { get; }
    public abstract Rarity rarity { get; }
    public abstract Tag tag { get; }
    public abstract BattleClass battleClass { get; } 
    public abstract byte height { get; }
    public abstract byte width { get;  }
    public abstract HealthComponent health { get; }
    public abstract Cooldown cooldown { get; }
    public abstract string description { get; }
    public abstract Element element { get; }
    public abstract List<Ability> abilities { get; }
    public bool playerSide = true;
    public uint[] statusEffects = new uint[Status.Amount];
    private bool dead;
    
    private bool isInInventory => InventoryManager.Instance != null 
                                  && InventoryManager.Instance.Contains(this);

    private bool isInBoard => BoardManager.Instance != null
                              && BoardManager.Instance.Contains(this);

    public void Restart()
    {
        health.health = (int) health.ogHealth;
        health.maxHealth = health.ogHealth;
        cooldown.timeSeconds = cooldown.ogTimeSeconds;
        cooldown.started = false;
        foreach (var a in abilities)
        {
            if (a == null) continue;
            a.bonusDamage = 0;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isInInventory && !isInBoard)
        {
            Destroy(gameObject);
            return;
        }
        
        bool isScene;
        if (isInBoard)
            isScene = scene.name == "CombatScene" || scene.name == "BossScene";
        else
            isScene = scene.name == "CombatScene" || scene.name == "StoreScene" || scene.name == "Reward" || scene.name == "BossScene";
        ChangeChildrenSortingLayer(isScene);
    }

    public void DoOnStart(Board allies, Board enemies)
    {
        cooldown.started = true;
        foreach (var ability in abilities)
            ability?.DoOnStart(allies, enemies);
        
        cooldown.Restart();
    }
    
    private void ChangeChildrenSortingLayer(bool show)
    {
        foreach (var rend in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            rend.enabled = show;
        }
    }

    public uint GetStatusAmount(Status.StatusEffect effect)
    {
        return statusEffects[(int)effect];
    }

    private void LowerStatusAmount(Status.StatusEffect effect)
    {
        statusEffects[(int)effect]--;
    }

    public void ApplyStatus(Status.StatusEffect effect, uint value)
    {
        if (effect == Status.StatusEffect.Burn)
            BattleManager.Instance.ApplyBurn.Invoke();
        OnApplyStatus(effect, value);
    }

    public void DoAbility(Board allies, Board enemies)
    {
        if (!cooldown.IsDone())
            return;
        
        foreach (var ability in abilities)
            ability?.DoAbility(allies, enemies);
        
        if (GetStatusAmount(Status.StatusEffect.Weak) > 0)
            LowerStatusAmount(Status.StatusEffect.Weak);

        cooldown.Restart();
    }

    private void Die()
    {
        if (dead)
            return;
        BattleManager.Instance.UnlogCreature(this);
        dead = true;
    }

    public void TakeDamage(uint damage)
    {
        if (dead)
            return;
        if (health.TakeDamage(damage))
            Die();
        CheckForRuin();
    }

    public void Heal(uint amount)
    {
        health.health += (int) amount;
        if (health.health > health.maxHealth)
            health.health = (int) health.maxHealth;
    }

    public void SetCooldownByRatio(float amount)
    {
        cooldown.timeSeconds *= amount;
        if (cooldown.timeSeconds < 0.5f) cooldown.timeSeconds = 0.5f;
        if (cooldown.timeSeconds > 16f) cooldown.timeSeconds = 16f;
    }
    

    public void DoOnTick()
    {
        if (GetStatusAmount(Status.StatusEffect.Burn) is uint value and > 0)
        {
            TakeDamage(value);
            LowerStatusAmount(Status.StatusEffect.Burn);
        }
    }

    private void CheckForRuin()
    {
        if (GetStatusAmount(Status.StatusEffect.Ruin) is uint value and > 0)
        {
            if (health.health <= value)
                Die();
        }
    }

    private void OnApplyStatus(Status.StatusEffect effect, uint value)
    {
        statusEffects[(int)effect] += value;
        CheckForRuin();
    }
}

public enum BattleClass
{
    Meele, Flank, AOE
}
