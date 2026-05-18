using UnityEngine;

public class CooldownBarComponent : MonoBehaviour
{
    [SerializeField] private Creature creature;
    [SerializeField] private Renderer rend;
    
    private void Update()
    {
        var amount = 0f;
        if (BattleManager.Instance.battleOngoing)
            amount = creature.cooldown.ElapsedTimeSec() / creature.cooldown.timeSeconds;
        rend.material.SetFloat("_Heigth", amount);
    }
}