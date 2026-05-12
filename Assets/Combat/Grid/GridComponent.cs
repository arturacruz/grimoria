using System;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GridComponent : MonoBehaviour
{
    public static readonly UnityEvent<GameObject, bool> CreatureSelected = new();
    public byte height = 3;
    public byte width = 3;
    public CreatureComponent[,] grid { get; private set; }

    [SerializeField] private GameObject tilePrefab;
    private float tileSize = 2f;
    private GameObject selectedCreature;
    
    private void Start()
    {
        grid = new CreatureComponent[height, width];

        for (var y = 0; y < height; y++)    
        {
            for (var x = 0; x < width; x++)
            {
                var tile = Instantiate(
                    tilePrefab,
                    GetWorldPosition(new Vector2Int(x, y)),
                    transform.rotation);
                tile.transform.SetParent(transform);
                tile.transform.localScale = new Vector3(tileSize, tileSize, 1);
            }
        }

        CreatureSelected.AddListener(OnCreatureSelected);
    }

    private Vector2Int GetMouseTilePosition()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        return GetTilePosition(mousePos);
    }

    private void Update()
    {
        if (selectedCreature == null)
            return;
        
        var tileMousePos = GetMouseTilePosition();
        
        var creature = selectedCreature.GetComponent<CreatureComponent>();

        if (!IsPositionInGrid(tileMousePos))
        {
            creature.canBePlaced = true;
            return;
        }
        
        creature.canBePlaced = false;
        for (var y = 0; y < creature.height; y++)
        {
            for (var x = 0; x < creature.width; x++)
            {
                var newTilePos = tileMousePos + new Vector2Int(x, y);
                if (!IsPositionInGrid(newTilePos) || IsPositionOccupied(newTilePos))
                    return;
            }
        }

        var tiledWorldPos = GetWorldPosition(tileMousePos);

        var creatureSizeOffset = new Vector2Int(creature.width - 1, creature.height - 1);
        selectedCreature.transform.localPosition = tiledWorldPos - creatureSizeOffset;
        creature.canBePlaced = true;
    }
    
    public Vector2 GetWorldPosition(Vector2Int tilePos)
    {
        var negTilePos = new Vector2Int(tilePos.x, -tilePos.y);
        // The tileSize / 2 is to correctly centralize the tiles. Rest is self-explanatory.
        return (Vector2) negTilePos * tileSize + (Vector2) transform.position + new Vector2(tileSize, tileSize) / 2;
    }

    public Vector2Int GetTilePosition(Vector2 worldPos)
    {
        var pos = (worldPos - (Vector2)transform.position) / tileSize;
        var x = (int) Math.Floor(pos.x);
        var y = (int) Math.Floor(pos.y);
        
        // This weird -y + height - 1 is to make the tilePos starting at the top left and go down to the bottom right.
        // That's how arrays work and Unity doesn't like it
        return new Vector2Int(x, -y);
    }

    public bool IsPositionOccupied(Vector2Int tilePos)
    {
        return grid[tilePos.y, tilePos.x] != null;
    }

    public bool IsPositionInGrid(Vector2Int tilePos)
    {
        return tilePos.x >= 0 && tilePos.x < width && tilePos.y >= 0 && tilePos.y < height;
    }

    private void OnCreatureSelected(GameObject creature, bool selected)
    {
        selectedCreature = creature;
        if (creature == null)
            return;
        
        selectedCreature = selected ? creature : null;
        
        var tile = GetTilePosition(creature.transform.position);
        if (!IsPositionInGrid(tile))
            return;

        var x = tile.x;
        var y = tile.y;

        var creatureComponent = creature.GetComponent<CreatureComponent>();
        var height = creatureComponent.height;
        var width = creatureComponent.width;

        CreatureComponent init = null;
        // It was deselected so it should be put in the grid
        if (!selected)
            init = creatureComponent;

        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                var pos = new Vector2Int(x + j, y + i);
                if (IsPositionInGrid(pos))
                    grid[pos.y, pos.x] = init;
            }
        }
    }
}
