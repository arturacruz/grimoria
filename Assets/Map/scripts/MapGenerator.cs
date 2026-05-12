using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] public int floors;
    [SerializeField] public int columns;
    [SerializeField] public float scale;
    [SerializeField] private float randomOffset = 1f;

    [SerializeField] private GameObject Casa_combate;
    [SerializeField] private GameObject linha;
    [SerializeField] private SpriteRenderer background;

    private Casa[,] matrix;

    private int startX;
    private int endX;

    private List<int> finalConnections = new List<int>();

    // guarda conexões por andar
    private Dictionary<int, List<(int from, int to)>> floorConnections =
        new Dictionary<int, List<(int, int)>>();

    private void Start()
    {
        matrix = new Casa[floors, columns];

        GenerateMap();
        ResizeBackground();
    }

    /// <summary>
    /// Calculates the world coordinate of the tile coordinate.
    /// </summary>
    public Vector2 GetRoomDrawPosition(int x, int y)
    {
        float xOffset =
            (x - (columns - 1) / 2.0f) * scale +
            Random.Range(-randomOffset, randomOffset);

        float yOffset =
            (y - (floors - 1) / 2.0f) * scale +
            Random.Range(-randomOffset, randomOffset);

        return new Vector2(xOffset, yOffset);
    }

    /// <summary>
    /// Adds a line between two Casas.
    /// </summary>
    private void AddLine(Casa previous, Casa current)
    {
        var line = Instantiate(linha).GetComponent<LineRenderer>();

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

        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        float mapWidth = columns * scale;
        float mapHeight = floors * scale;

        float finalWidth = Mathf.Max(screenWidth, mapWidth);
        float finalHeight = Mathf.Max(screenHeight, mapHeight);

        background.size = new Vector2(finalWidth, finalHeight + 2);

        background.transform.position = new Vector3(0, 0, 1);
    }

    /// <summary>
    /// Initializes all rooms.
    /// </summary>
    private void InitializeRooms()
    {
        for (int y = 0; y < floors; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var room = Instantiate(
                    Casa_combate,
                    GetRoomDrawPosition(x, y),
                    Quaternion.identity
                );

                matrix[y, x] = room.GetComponent<Casa>();
            }
        }
    }

    /// <summary>
    /// Checks if a connection crosses another.
    /// </summary>
    private bool ConnectionCrosses(int floor, int fromX, int toX)
    {
        if (!floorConnections.ContainsKey(floor))
            return false;

        foreach (var connection in floorConnections[floor])
        {
            bool crosses =
                (fromX < connection.from && toX > connection.to) ||
                (fromX > connection.from && toX < connection.to);

            if (crosses)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Registers a connection.
    /// </summary>
    private void RegisterConnection(int floor, int fromX, int toX)
    {
        if (!floorConnections.ContainsKey(floor))
        {
            floorConnections[floor] =
                new List<(int, int)>();
        }

        floorConnections[floor].Add((fromX, toX));
    }

    /// <summary>
    /// Generates one path.
    /// </summary>
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
            {
                previous.lista_casa.Add(current);
            }

            AddLine(previous, current);

            previous = current;
        }

        finalConnections.Add(x);
    }

    /// <summary>
    /// Generates all paths.
    /// </summary>
    private void GeneratePaths()
    {
        startX = columns / 2;

        Casa starting = matrix[0, startX];

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
            Mathf.RoundToInt((float)sum / finalConnections.Count);

        Casa endRoom = matrix[floors - 1, endX];

        foreach (int x in finalConnections)
        {
            Casa previous = matrix[floors - 2, x];

            if (previous == null)
                continue;

            if (!previous.lista_casa.Contains(endRoom))
            {
                previous.lista_casa.Add(endRoom);
            }

            AddLine(previous, endRoom);
        }
    }

    /// <summary>
    /// Categorizes rooms.
    /// </summary>
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

                // Boss
                if (y == floors - 1 && x == endX)
                {
                    sprite.color = Color.red;
                    room.tipo_casa = CategoriaCasa.Boss;
                    continue;
                }

                // remove unused
                if (room.lista_casa.Count == 0)
                {
                    Destroy(room.gameObject);
                    matrix[y, x] = null;
                    continue;
                }

                // start
                if (y == 0)
                {
                    sprite.color = Color.magenta;
                    continue;
                }

                // shop
                if (room.tipo_casa == CategoriaCasa.Shop)
                {
                    sprite.color = Color.green;

                    foreach (Casa nextRoom in room.lista_casa)
                    {
                        if (nextRoom.tipo_casa ==
                            CategoriaCasa.Shop)
                        {
                            nextRoom.tipo_casa =
                                CategoriaCasa.Combate;

                            SpriteRenderer nextSprite =
                                nextRoom.GetComponent<SpriteRenderer>();

                            nextSprite.color = Color.yellow;
                        }
                    }
                }

                // combat
                else if (room.tipo_casa ==
                         CategoriaCasa.Combate)
                {
                    sprite.color = Color.yellow;
                }
            }
        }
    }
}