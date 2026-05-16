using UnityEngine;

public class RuinBarComponent : MonoBehaviour
{
    [SerializeField] private Creature creature;
    [SerializeField] private Renderer rend;

    private void Update()
    {
        rend.material.SetFloat("_Heigth", creature.GetStatusAmount(Status.StatusEffect.Ruin) / (float) creature.health.maxHealth);
    }
}
