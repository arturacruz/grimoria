using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private byte floors = 15;
    [SerializeField] private byte columns = 5;
    [SerializeField] private byte paths = 3;
    [SerializeField] private float scale = 50;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject linePrefab;

    private Cell[,] matrix;

    private void Start()
    {
        matrix = new Cell[floors, columns];
        GenerateMap();
    }

    /// <summary>
    /// Calculates the world coordinate of the "x, y" tile coordinate. 
    /// </summary>
    private Vector2 GetRoomDrawPosition(int x, int y)
    {
        // This math is only to center the map as much as possible for the camera
        var xOffset = (x - (columns - 1) / 2.0f) * scale;
        var yOffset = (y - (floors - 1) / 2.0f) * scale;
        return new Vector2(xOffset, yOffset);
    }

    /// <summary>
    /// Adds a line between two cells.
    /// </summary>
    private void AddLine(Cell previous, Cell current)
    {
        var line = Instantiate(linePrefab).GetComponent<LineRenderer>();
        Vector3[] linePositions = {
            previous.transform.position,
            current.transform.position
        };
            
        line.SetPositions(linePositions);
    }
    
    private void GenerateMap()
    {
        // var initialPos = Instantiate(cellPrefab);
        // var endPos = Instantiate(cellPrefab);
        
        InitializeRooms();
        GeneratePaths();
        CategorizeRooms();
    }
    
    /// <summary>
    /// Initializes the [floors, columns] matrix with newly instantiated cells.
    /// </summary>
    private void InitializeRooms()
    {
        for (var y = 0; y < floors; y++)
        {
            for (var x = 0; x < columns; x++)
            {
                var room = Instantiate(
                    cellPrefab,
                    GetRoomDrawPosition(x, y),
                    transform.rotation);
                
                matrix[y, x] = room.GetComponent<Cell>();
            }
        }
    }

    /// <summary>
    /// Determines randomly which of the columns will have a starting path.
    /// </summary>
    private bool[] GenerateFirstFloorChoices()
    {
        var firstFloorChoices = new bool[columns];
        for (var i = 0; i < paths; i++)
        {
            var randRoom = Random.Range(0, columns);

            // Make sure the second is not equal to the first, guaranteeing at least two starting paths.
            while (i == 1 && firstFloorChoices[randRoom])
                randRoom = Random.Range(0, columns);

            firstFloorChoices[randRoom] = true;
        }

        return firstFloorChoices;
    }

    /// <summary>
    /// Continues generating and traversing the starting paths until the end.
    /// </summary>
    private void TraversePath(int x, Cell previous)
    {
        for (var y = 1; y < floors; y++)
        {
            var minX = Mathf.Max(0, x - 1);
            var maxX = Mathf.Min(x + 1, columns - 1);

            x = Random.Range(minX, maxX + 1);
            var current = matrix[y, x];
            // Not to duplicate connections
            if (!previous.Connections.Contains(current))
                previous.Connections.Add(current);

            AddLine(previous, current);

            previous = current;
        }
    }

    /// <summary>
    /// General function that determines starting paths and develops each one until the end of the map.
    /// </summary>
    private void GeneratePaths()
    {
        var firstFloorChoices = GenerateFirstFloorChoices();

        for (var x = 0; x < columns; x++)
        {
            if (!firstFloorChoices[x])
                continue;
            
            Debug.Log(x);
            
            var starting = matrix[0, x];
            TraversePath(x, starting);
        }
    }

    /// <summary>
    /// Deletes rooms without connections and categorizes them randomly between the available types.
    /// </summary>
    private void CategorizeRooms()
    {
        // TODO: Categorize randomly. For now this only deletes.
        for (var y = 0; y < floors; y++)
        {
            for (var x = 0; x < columns; x++)
            {
                var room = matrix[y, x];
                if (room.Connections.Count != 0)
                    continue;
                
                Destroy(room.gameObject);
                matrix[y, x] = null;
            }
        }
    }
}
