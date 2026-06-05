using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    public List<GameObject> creatures;
    public GameObject board;

    private void Awake()
    {
        if (Instance != null)
        {
            var first = FindObjectsByType<BoardManager>(FindObjectsSortMode.InstanceID);
            foreach (var f in first)
            {
                if (f != Instance)
                    Destroy(f);
            }
        }

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
            if (board != null)
                DontDestroyOnLoad(board);
        }
    }

    public void DestroyForNewRun()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (board != null)
            Destroy(board);

        foreach (var creature in creatures)
        {
            if (creature != null)
                Destroy(creature);
        }

        creatures.Clear();
        if (Instance == this)
            Instance = null;

        Destroy(gameObject);
    }
    
    public bool Contains(Creature creature)
    {
        return creatures.Contains(creature.gameObject);
    }
    
    public void AddToBoardManager(Creature creature)
    {
        if (creatures.Contains(creature.gameObject))
            return;

        creatures.Add(creature.gameObject);
    }

    public void RemoveFromBoardManager(Creature creature)
    {
        creatures.Remove(creature.gameObject);
    }
    
    public void Show(bool visible)
    {
        foreach (var c in creatures)
        { 
            c.GetComponent<Creature>().Restart();
            foreach (var rend in c.GetComponentsInChildren<SpriteRenderer>()) rend.enabled = visible;
            
            c.layer = visible ? 0 : 2;
            c.SetActive(visible);
        }

        foreach (var g in board.GetComponentsInChildren<SpriteRenderer>()) g.enabled = visible;
        board.layer = visible ? 0 : 2;
        gameObject.layer = visible ? 0 : 2;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var isScene = scene.name == "CombatScene" || scene.name == "BossScene";
        Show(isScene);
        if (isScene)
        {
            BattleManager.Instance.player = board.GetComponent<Board>();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this)
            Instance = null;
    }
}
