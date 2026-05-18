using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public GameObject grid;
    public List<GameObject> creatures;
    public uint money;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            if (grid != null)
                DontDestroyOnLoad(grid);
        }
    }

    public bool Contains(Creature creature)
    {
        return creatures.Contains(creature.gameObject);
    }

    public void AddToInventory(Creature creature)
    {
        creatures.Add(creature.gameObject);
    }

    public void RemoveFromInventory(Creature creature)
    {
        creatures.Remove(creature.gameObject);
    }

    public void Init(bool visible)
    {
        foreach (var c in creatures)
            c.GetComponent<SpriteRenderer>().enabled = visible;

        grid.GetComponent<SpriteRenderer>().enabled = visible;
    }
}