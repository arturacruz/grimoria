using UnityEngine;

public class LifeBarComponent : MonoBehaviour
{
    [SerializeField] private Creature creature;
    [SerializeField] private Renderer rend;

    
    private void Update()
    {
        rend.material.SetFloat("_Heigth", creature.health.health / (float) creature.health.maxHealth);
    }
}