using UnityEngine;

public class LifeBarComponent : MonoBehaviour
{
    private uint maxHealth;

    [SerializeField] private Renderer rend;

    public void Init(uint maxHealth)
    {
        this.maxHealth = maxHealth;
    }
    
    public void UpdateValue(int health)
    {
        rend.material.SetFloat("_Heigth", health / (float) maxHealth);
    }
}