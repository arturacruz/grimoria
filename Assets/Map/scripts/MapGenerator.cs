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

    private void Start()
    {
        matrix = new Casa[floors, columns];
        SpriteRenderer color_casa = GetComponent<SpriteRenderer>();

        GenerateMap();
        ResizeBackground();
    }

    /// <summary>
    /// Calculates the world coordinate of the "x, y" tile coordinate. 
    /// </summary>
    public Vector2 GetRoomDrawPosition(int x, int y)
    {
        // This math is only to center the map as much as possible for the camera
        var xOffset = (x - (columns - 1) / 2.0f) * scale + Random.Range(-randomOffset, randomOffset);
        var yOffset = (y - (floors - 1) / 2.0f) * scale + Random.Range(-randomOffset, randomOffset);;
        return new Vector2(xOffset, yOffset);
    }

    /// <summary>
    /// Adds a line between two Casas.
    /// </summary>
    private void AddLine(Casa previous, Casa current)
    {
        var line = Instantiate(linha).GetComponent<LineRenderer>();
        Vector3[] linePositions = {
            previous.transform.position,
            current.transform.position
        };
            
        line.SetPositions(linePositions);

    }
    
    private void GenerateMap()
    {
        // var initialPos = Instantiate(Casa_combate);
        // var endPos = Instantiate(Casa_combate);
        
        InitializeRooms();
        GeneratePaths();
        CategorizeRooms();
    }

    private void ResizeBackground()
    {
        background.sortingOrder = -10;

        SpriteRenderer sr = background;

        // Tamanho da tela (câmera)
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        // Tamanho do mapa
        float mapWidth = columns * scale;
        float mapHeight = floors * scale;

        // Usa o maior dos dois
        float finalWidth = Mathf.Max(screenWidth, mapWidth);
        float finalHeight = Mathf.Max(screenHeight, mapHeight);

        sr.size = new Vector2(finalWidth, finalHeight + 2);

        background.transform.position = new Vector3(0, 0, 1);
    }
    
    /// <summary>
    /// Initializes the [floors, columns] matrix with newly instantiated Casas.
    /// </summary>
    private void InitializeRooms()
    {
        for (var y = 0; y < floors; y++)
        {
            for (var x = 0; x < columns; x++)
            {
                var room = Instantiate(
                    Casa_combate,
                    GetRoomDrawPosition(x, y),
                    transform.rotation);
                
                matrix[y, x] = room.GetComponent<Casa>();
            }
        }
    }

    /// <summary>
    /// Continues generating and traversing the starting paths until the end.
    /// </summary>
    private void TraversePath(int x, Casa previous)
    {
        for (var y = 1; y < floors; y++)
        {
            if (y == floors - 1)
            {
                x = endX;
            }
            
            else
            {
                var minX = Mathf.Max(0, x - 1);
                var maxX = Mathf.Min(x + 1, columns - 1);
                x = Random.Range(minX, maxX + 1);
            }

            var current = matrix[y, x];

            if (!previous.lista_casa.Contains(current))
                previous.lista_casa.Add(current);

            AddLine(previous, current);
            previous = current;
        }
    }

    /// <summary>
    /// General function that determines starting paths and develops each one until the end of the map.
    /// </summary>
    private void GeneratePaths()
    {
        startX = columns/2;
        endX = columns/2;

        var starting = matrix[0, startX];

        for (var x = 0; x < columns; x++)
        {
            TraversePath(x, starting);
        }
    }

    /// <summary>
    /// Deletes rooms without lista_casa and categorizes them randomly between the available types.
    /// </summary>
    private void CategorizeRooms()
    {
        // TODO: Categorize randomly. For now this only deletes.
        for (var y = 0; y < floors; y++)
        {
            for (var x = 0; x < columns; x++) {
                var room = matrix[y, x];

                var sprite = room.GetComponent<SpriteRenderer>();
                if (y == floors-1) {
                    sprite.color = Color.red;
                }
                
                if (room.lista_casa.Count == 0 && !(y == floors - 1 && x == endX)) {
                    Destroy(room.gameObject);
                    matrix[y, x] = null;
                }
            }
        }
    }
}