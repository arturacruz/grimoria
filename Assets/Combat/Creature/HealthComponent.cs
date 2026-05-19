using UnityEngine;

public class HealthComponent
{
    public uint ogHealth;
    public uint maxHealth;
    public int health;
    
    public HealthComponent(uint maxHealth)
    {
        ogHealth = maxHealth;
        this.maxHealth = maxHealth;
        health = (int) maxHealth;
    }

    private bool ShouldDie()
    {
        return health <= 0;
    }

    public bool TakeDamage(uint damage)
    {
        health -= (int) damage;
        return ShouldDie();
    }
    
}