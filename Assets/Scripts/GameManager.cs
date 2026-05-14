using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject SelectedCreature;
    public GameObject HoveringCreature;
    public bool CanBePlaced;
    public UnityEvent<bool> PlaceCreature = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}
