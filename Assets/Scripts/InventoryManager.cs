using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public GameObject grid;
    public List<GameObject> creatures;
    public uint money;

    private void Awake()
    {
        if (Instance != null)
        {
            var first = FindObjectsByType<InventoryManager>(FindObjectsSortMode.InstanceID);
            foreach (var f in first)
            {
                Debug.Log(f.gameObject);
            }
        }
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
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

    public void Show(bool visible)
    {
        foreach (var c in creatures)
        { 
            foreach (var rend in c.GetComponentsInChildren<SpriteRenderer>()) rend.enabled = visible;
            c.gameObject.layer = visible ? 0 : 2;
        }

        foreach (var g in grid.GetComponents<SpriteRenderer>()) g.enabled = visible;
        grid.layer = visible ? 0 : 2;
        gameObject.layer = visible ? 0 : 2;
    }
    

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var isScene = scene.name == "CombatScene" || scene.name == "StoreScene" || scene.name == "Reward";
        Show(isScene);
    }

}