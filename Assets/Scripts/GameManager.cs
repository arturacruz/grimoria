using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject SelectedCreature;

    public bool CanBePlaced;

    public UnityEvent<bool> PlaceCreature = new();
    public CategoriaCasa[,] roomStates;

    public bool[,] visitedRooms;

    public int currentX;
    public int currentY;

    public bool mapGenerated = false;

    public int floors;
    public int columns;
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
}