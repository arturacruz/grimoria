using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;


public class MapGenerator : MonoBehaviour
{
    [SerializeField] public int floors;
    [SerializeField] public int columns;
    [SerializeField] public float scale = 7;

    [SerializeField] private float horizontalSpacing = 8f;
    [SerializeField] private float verticalSpacing = 6f;
    [SerializeField] private float randomOffset = 0.47f;

    [SerializeField] private GameObject Casa_combate;
    [SerializeField] private GameObject linha;
    [SerializeField] private SpriteRenderer background;

    [SerializeField] public Sprite combat_sprite;
    [SerializeField] public Sprite store_sprite;
    [SerializeField] public Sprite boss_sprite;
    [SerializeField] public Sprite start_sprite;

    [SerializeField] private Material material_casa;
    [SerializeField] private SpriteRenderer player;

    public static SpriteRenderer Player;
    public static Casa casa_atual;
    public static MapGenerator Instance;

    public bool active;

    private Casa[,] matrix;
    private int startX;
    private int endX;

    private bool mapGenerated = false;

    private List<int> finalConnections = new();
    private Dictionary<int, List<(int from, int to)>> floorConnections =
        new Dictionary<int, List<(int, int)>>();

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

    public void DestroyForNewRun()
    {
        if (Instance == this)
            Instance = null;
        if (Player == player)
            Player = null;
        if (casa_atual != null && casa_atual.transform.IsChildOf(transform))
            casa_atual = null;

        Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (mapGenerated)
            return;

        Player = player;

        player.sortingLayerName = "Map";
        player.sortingOrder = 15;

        Player.transform.localScale = Vector3.one * 0.47f;
        Player.transform.SetParent(transform);

        matrix = new Casa[floors, columns];

        GenerateMap();
        ResizeBackground();

        mapGenerated = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isMapScene = scene.name == "MapScene";
        SetMapLayer(isMapScene);            
    }

    private void SetAlpha(SpriteRenderer sr, bool active)
    {
        Color c = sr.color;
        c.a = active ? 1f : 0f;
        sr.color = c;
    }

    private void SetMapLayer(bool active)
    {
        this.active = active;
        string layer = "Map";

        background.GetComponent<SpriteRenderer>().enabled = active;

        player.GetComponent<SpriteRenderer>().enabled = active;
        SetAlpha(player, active);

        foreach (Transform child in transform)
        {
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = layer;
                sr.GetComponent<SpriteRenderer>().enabled = active;
                child.gameObject.layer = active ? 0:2;
                SetAlpha(sr, active);
            }

            Transform mark = child.Find("mark");

            if (mark != null)
            {
                SpriteRenderer marcado = mark.GetComponent<SpriteRenderer>();

                if (marcado != null)
                {
                    marcado.enabled = active;

                    var casa = child.GetComponent<Casa>();

                    if (casa.tipo_casa != CategoriaCasa.Visitada)
                    {
                        marcado.enabled = false;
                    }
                }
            }

            var lr = child.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.enabled = active;
            }
        }
    }

    public Vector2 GetRoomDrawPosition(int x, int y)
    {
        var basePosition = GetRoomBasePosition(x, y);
        float xPos = basePosition.x;
        float yPos = basePosition.y;

        xPos += Random.Range(-randomOffset, randomOffset);
        yPos += Random.Range(-randomOffset, randomOffset);

        return new Vector2(xPos, yPos);
    }

    public Vector2 GetRoomBasePosition(int x, int y)
    {
        float xPos = (x - (columns - 1) / 2f) * horizontalSpacing;
        float yPos = (y - (floors - 1) / 2f) * verticalSpacing;

        return new Vector2(xPos, yPos);
    }

    private void AddLine(Casa previous, Casa current)
    {
        var line = Instantiate(linha, transform).GetComponent<LineRenderer>();

        line.SetPositions(new Vector3[]
        {
            previous.transform.position,
            current.transform.position
        });
    }

    private void GenerateMap()
    {
        InitializeRooms();
        GeneratePaths();
        CategorizeRooms();
    }

    private void ResizeBackground()
    {
        background.sortingOrder = -10;

        player.sortingOrder = 100;

        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        float mapWidth = columns * scale;
        float mapHeight = floors * scale;

        background.size = new Vector2(
            Mathf.Max(screenWidth, mapWidth),
            Mathf.Max(screenHeight, mapHeight) + 20
        );

        background.transform.position = new Vector3(0, 0, 1);
    }

    private void InitializeRooms()
    {
        for (int y = 0; y < floors; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var room = Instantiate(
                    Casa_combate,
                    GetRoomDrawPosition(x, y),
                    Quaternion.identity,
                    transform
                );

                room.transform.localScale = Vector3.one * 0.47f;

                matrix[y, x] = room.GetComponent<Casa>();
            }
        }
    }

    private bool ConnectionCrosses(int floor, int fromX, int toX)
    {
        if (!floorConnections.ContainsKey(floor))
            return false;

        foreach (var c in floorConnections[floor])
        {
            if ((fromX < c.from && toX > c.to) ||
                (fromX > c.from && toX < c.to))
                return true;
        }

        return false;
    }

    private void RegisterConnection(int floor, int fromX, int toX)
    {
        if (!floorConnections.ContainsKey(floor))
            floorConnections[floor] = new List<(int, int)>();

        floorConnections[floor].Add((fromX, toX));
    }

    private void TraversePath(int x, Casa previous)
    {
        for (int y = 1; y < floors - 1; y++)
        {
            int minX = Mathf.Max(0, x - 1);
            int maxX = Mathf.Min(columns - 1, x + 1);

            int nextX;
            do
            {
                nextX = Random.Range(minX, maxX + 1);
            }
            while (ConnectionCrosses(y, x, nextX));

            RegisterConnection(y, x, nextX);
            x = nextX;

            Casa current = matrix[y, x];

            if (!previous.lista_casa.Contains(current))
                previous.lista_casa.Add(current);

            AddLine(previous, current);
            previous = current;
        }

        finalConnections.Add(x);
    }

    private void GeneratePaths()
    {
        startX = columns / 2;

        Casa starting = matrix[0, startX];

        for (int x = 0; x < columns; x++)
            TraversePath(x, starting);

        int sum = 0;
        foreach (int v in finalConnections)
            sum += v;

        endX = Mathf.RoundToInt((float)sum / finalConnections.Count);

        Casa endRoom = matrix[floors - 1, endX];

        foreach (int x in finalConnections)
        {
            Casa previous = matrix[floors - 2, x];

            if (previous == null)
                continue;

            if (!previous.lista_casa.Contains(endRoom))
                previous.lista_casa.Add(endRoom);

            AddLine(previous, endRoom);
        }
    }

    private void CategorizeRooms()
    {
        for (int y = 0; y < floors; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Casa room = matrix[y, x];
                if (room == null) continue;

                var sprite = room.GetComponent<SpriteRenderer>();
                sprite.material = material_casa;

                if (y == 0 && x == startX)
                {
                    sprite.sprite = start_sprite;
                    room.tipo_casa = CategoriaCasa.Inicio;
                    casa_atual = room;

                    player.transform.position =
                        new Vector3(room.transform.position.x, room.transform.position.y, -1f);

                    continue;
                }

                if (y == floors - 1 && x == endX)
                {
                    sprite.sprite = boss_sprite;
                    room.tipo_casa = CategoriaCasa.Boss;
                    room.transform.localScale = Vector3.one * 0.7f;
                    continue;
                }

                if (room.lista_casa.Count == 0)
                {
                    Destroy(room.gameObject);
                    matrix[y, x] = null;
                    continue;
                }

                if (Random.value < 0.2f)
                {
                    room.tipo_casa = CategoriaCasa.Shop;
                    sprite.sprite = store_sprite;
                    room.gameObject.name = "Loja";
                }
                else
                {
                    room.tipo_casa = CategoriaCasa.Combate;
                    sprite.sprite = combat_sprite;
                    room.gameObject.name = "Combate";
                }
            }
        }
    }
}
