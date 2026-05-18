using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GridComponent grid;
    public uint money = 100;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool TrySpend(uint amount)
    {
        if (money < amount)
            return false;

        money -= amount;
        return true;
    }

    public void AddMoney(uint amount)
    {
        money += amount;
    }

    public uint GetPrice(Creature creature)
    {
        return ((uint)creature.rarity + 1u) * 50u;
    }
}