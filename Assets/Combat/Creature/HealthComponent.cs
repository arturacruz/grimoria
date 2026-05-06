using UnityEngine;

public class HealthComponent
{
    public uint maxHealth;
    public uint health;
    
    public HealthComponent(uint maxHealth)
    {
        this.maxHealth = maxHealth;
        health = maxHealth;
    }
    
}