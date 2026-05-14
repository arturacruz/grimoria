using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] public int floors;
    [SerializeField] public int columns;

    [SerializeField] public float scale = 7;

    [SerializeField] private float horizontalSpacing = 8f;
    [SerializeField] private float verticalSpacing = 6f;

    [SerializeField] private float randomOffset = 0.47f;

    [Header("Prefabs")]
    [SerializeField] private GameObject Casa_combate;
    [SerializeField] private GameObject linha;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer background;

    [SerializeField] public Sprite combat_sprite;
    [SerializeField] public Sprite store_sprite;
    [SerializeField] public Sprite boss_sprite;
    [SerializeField] public Sprite start_sprite;

    [Header("Materials")]
    [SerializeField] private Material material_casa;

    [Header("Player")]
    [SerializeField] private SpriteRenderer player;

    public static SpriteRenderer Player;

    public static Casa casa_atual;

    private Casa[,] matrix;

    private int startX;
    private int endX;

    private List<int> finalConnections =
        new List<int>();

    private Dictionary<int, List<(int from, int to)>> floorConnections =
        new Dictionary<int, List<(int, int)>>();


    private void Start()
    {
        Player = player;

        Player.transform.localScale =
            Vector3.one * 0.47f;

        matrix = new Casa[floors, columns];

        GenerateMap();

        if (GameManager.Instance.mapGenerated)
        {
            RestoreMapState();
        }

        ResizeBackground();
    }


    public Vector2 GetRoomDrawPosition(int x, int y)
    {
        float xPos =
            (x - (columns - 1) / 2f) *
            horizontalSpacing;

        float yPos =
            (y - (floors - 1) / 2f) *
            verticalSpacing;

        xPos +=
            Random.Range(
                -randomOffset,
                randomOffset
            );

        yPos +=
            Random.Range(
                -randomOffset,
                randomOffset
            );

        return new Vector2(xPos, yPos);
    }


    private void AddLine(Casa previous, Casa current)
    {
        var line =
            Instantiate(linha)
            .GetComponent<LineRenderer>();

        Vector3[] linePositions =
        {
            previous.transform.position,
            current.transform.position
        };

        line.SetPositions(linePositions);
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

        float screenHeight =
            Camera.main.orthographicSize * 2;

        float screenWidth =
            screenHeight *
            Screen.width /
            Screen.height;

        float mapWidth =
            columns * scale;

        float mapHeight =
            floors * scale;

        float finalWidth =
            Mathf.Max(screenWidth, mapWidth);

        float finalHeight =
            Mathf.Max(screenHeight, mapHeight);

        background.size =
            new Vector2(
                finalWidth,
                finalHeight + 20
            );

        background.transform.position =
            new Vector3(0, 0, 1);
    }


    private void InitializeRooms()
    {
        for (int y = 0; y < floors; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var room =
                    Instantiate(
                        Casa_combate,
                        GetRoomDrawPosition(x, y),
                        Quaternion.identity
                    );

                room.transform.localScale =
                    Vector3.one * 0.47f;

                Casa casa =
                    room.GetComponent<Casa>();

                casa.x = x;
                casa.y = y;

                matrix[y, x] = casa;
            }
        }
    }


    private bool ConnectionCrosses(
        int floor,
        int fromX,
        int toX
    )
    {
        if (!floorConnections.ContainsKey(floor))
            return false;

        foreach (var connection in floorConnections[floor])
        {
            bool crosses =
                (fromX < connection.from &&
                 toX > connection.to)
                ||
                (fromX > connection.from &&
                 toX < connection.to);

            if (crosses)
                return true;
        }

        return false;
    }


    private void RegisterConnection(
        int floor,
        int fromX,
        int toX
    )
    {
        if (!floorConnections.ContainsKey(floor))
        {
            floorConnections[floor] =
                new List<(int, int)>();
        }

        floorConnections[floor]
            .Add((fromX, toX));
    }


    private void TraversePath(
        int x,
        Casa previous
    )
    {
        for (int y = 1; y < floors - 1; y++)
        {
            int minX =
                Mathf.Max(0, x - 1);

            int maxX =
                Mathf.Min(columns - 1, x + 1);

            int nextX;

            do
            {
                nextX =
                    Random.Range(
                        minX,
                        maxX + 1
                    );
            }
            while (
                ConnectionCrosses(
                    y,
                    x,
                    nextX
                )
            );

            RegisterConnection(y, x, nextX);

            x = nextX;

            Casa current =
                matrix[y, x];

            if (!previous.lista_casa.Contains(current))
            {
                previous.lista_casa.Add(current);
            }

            AddLine(previous, current);

            previous = current;
        }

        finalConnections.Add(x);
    }


    private void GeneratePaths()
    {
        startX = columns / 2;

        Casa starting =
            matrix[0, startX];

        for (int x = 0; x < columns; x++)
        {
            TraversePath(x, starting);
        }

        int sum = 0;

        foreach (int value in finalConnections)
        {
            sum += value;
        }

        endX =
            Mathf.RoundToInt(
                (float)sum /
                finalConnections.Count
            );

        Casa endRoom =
            matrix[floors - 1, endX];

        foreach (int x in finalConnections)
        {
            Casa previous =
                matrix[floors - 2, x];

            if (previous == null)
                continue;

            if (!previous.lista_casa.Contains(endRoom))
            {
                previous.lista_casa.Add(endRoom);
            }

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

                if (room == null)
                    continue;

                SpriteRenderer sprite =
                    room.GetComponent<SpriteRenderer>();

                sprite.material =
                    material_casa;


                if (y == 0 && x == startX)
                {
                    sprite.sprite =
                        start_sprite;

                    room.gameObject.name =
                        "Inicio";

                    room.tipo_casa =
                        CategoriaCasa.Inicio;

                    casa_atual = room;

                    player.transform.position =
                        new Vector3(
                            room.transform.position.x,
                            room.transform.position.y,
                            -1f
                        );

                    player.sortingOrder = 100;

                    continue;
                }


                if (y == floors - 1 &&
                    x == endX)
                {
                    sprite.sprite =
                        boss_sprite;

                    room.tipo_casa =
                        CategoriaCasa.Boss;

                    room.gameObject.name =
                        "Boss";

                    room.transform.localScale =
                        Vector3.one * 0.7f;

                    continue;
                }


                if (room.lista_casa.Count == 0)
                {
                    Destroy(room.gameObject);

                    matrix[y, x] = null;

                    continue;
                }


                if (room.tipo_casa ==
                    CategoriaCasa.Shop)
                {
                    sprite.sprite =
                        store_sprite;

                    room.gameObject.name =
                        "Loja";

                    foreach (
                        Casa nextRoom
                        in room.lista_casa
                    )
                    {
                        if (
                            nextRoom.tipo_casa ==
                            CategoriaCasa.Shop
                        )
                        {
                            nextRoom.tipo_casa =
                                CategoriaCasa.Combate;

                            SpriteRenderer nextSprite =
                                nextRoom.GetComponent<SpriteRenderer>();

                            nextSprite.sprite =
                                combat_sprite;
                        }
                    }
                }


                else
                {
                    sprite.sprite =
                        combat_sprite;

                    room.gameObject.name =
                        "Combate";

                    room.tipo_casa =
                        CategoriaCasa.Combate;
                }
            }
        }

        SaveMapState();
    }


    private void SaveMapState()
    {
        GameManager.Instance.floors =
            floors;

        GameManager.Instance.columns =
            columns;

        GameManager.Instance.roomStates =
            new CategoriaCasa[floors, columns];

        GameManager.Instance.visitedRooms =
            new bool[floors, columns];

        for (int y = 0; y < floors; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Casa room = matrix[y, x];

                if (room == null)
                    continue;

                GameManager.Instance.roomStates[y, x] =
                    room.tipo_casa;

                GameManager.Instance.visitedRooms[y, x] =
                    room.visited;
            }
        }

        GameManager.Instance.mapGenerated = true;
    }


    private void RestoreMapState()
    {
        for (int y = 0; y < floors; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Casa room = matrix[y, x];

                if (room == null)
                    continue;

                room.tipo_casa =
                    GameManager.Instance.roomStates[y, x];

                room.visited =
                    GameManager.Instance.visitedRooms[y, x];

                SpriteRenderer sprite =
                    room.GetComponent<SpriteRenderer>();

                if (room.visited)
                {
                    sprite.color = Color.white;

                    room.gameObject.name =
                        "Visitada";
                }
            }
        }

        Casa currentRoom =
            matrix[
                GameManager.Instance.currentY,
                GameManager.Instance.currentX
            ];

        if (currentRoom != null)
        {
            casa_atual = currentRoom;

            player.transform.position =
                currentRoom.transform.position;
        }
    }
}