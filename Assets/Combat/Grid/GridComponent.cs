using System;
using Combat.BoardPreset;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GridComponent : MonoBehaviour
{
    [SerializeField] private BoardPresetObject boardPreset;
    public byte height { get; private set; }= 3;
    public byte width { get; private set; } = 3;

    public Creature[,] grid { get; private set; }
    public bool[,] occupances { get; private set; }
    private bool isEnemy;
    [SerializeField] private bool isInventory;
    [SerializeField] private bool isPlayerBoard;
    [SerializeField] private bool isShop;
    [SerializeField] private GameObject tilePrefab;
    private float tileSize = 2f;
    private GameObject selectedCreature => GameManager.Instance.SelectedCreature;

    private bool canCreatureBePlaced
    {
        get => GameManager.Instance.CanBePlaced;
        set => GameManager.Instance.CanBePlaced = value;
    }

    public void SetAsEnemy()
    {
        isEnemy = true;
    }
    
    private void Start()
    {
        if (boardPreset != null)
        {
            height = (byte) boardPreset.preset.Length;
            width = (byte) boardPreset.preset[0].creatures.Length;
        }
        grid = new Creature[height, width];
        occupances = new bool[height, width];

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
                if (boardPreset == null) continue;

                var creature = boardPreset.preset[y].creatures[x];
                if (creature == null) continue;
                
                PlaceCreature(x, y, creature);
            }
        }
        
        GameManager.Instance.PlaceCreature.AddListener(OnPlaceCreature);
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
        if (isEnemy)
            return;
        if (selectedCreature == null)
            return;
        
        var tileMousePos = GetMouseTilePosition();
        var creature = selectedCreature.GetComponent<Creature>();

        // If the creature is not on this grid
        if (!IsPositionInGrid(tileMousePos))
            return;
        
        for (var y = 0; y < creature.height; y++)
        {
            for (var x = 0; x < creature.width; x++)
            {
                var newTilePos = tileMousePos + new Vector2Int(- x, y);
                if (!IsPositionInGrid(newTilePos) || IsPositionOccupied(newTilePos))
                    return;
            }
        }
        
        var tiledWorldPos = GetWorldPosition(tileMousePos);

        var creatureSizeOffset = new Vector2Int(creature.width - 1, creature.height - 1);
        selectedCreature.transform.localPosition = tiledWorldPos - creatureSizeOffset;
        canCreatureBePlaced = true;
    }
    
    private Vector2 GetWorldPosition(Vector2Int tilePos)
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

    private bool IsPositionOccupied(Vector2Int tilePos)
    {
        return occupances[tilePos.y, tilePos.x];
    }

    private void PlaceCreature(int x, int y, Creature creature)
    {
        var creatureSizeOffset = new Vector2Int(creature.width - 1, creature.height - 1);
        var obj = Instantiate(
            creature.gameObject,
            GetWorldPosition(new Vector2Int(x, y)) - creatureSizeOffset,
            transform.rotation);
        var c = obj.GetComponent<Creature>();

        c.playerSide = !isEnemy;
        grid[y, x] = c; 
        for (var i = 0; i < creature.height; i++)
            for (var j = 0; j < creature.width; j++)
                occupances[y + i, x + -j] = true;
    }
    
    private bool IsPositionInGrid(Vector2Int tilePos)
    {
        return tilePos.x >= 0 && tilePos.x < width && tilePos.y >= 0 && tilePos.y < height;
    }

    private void OnPlaceCreature(bool placed)
    {
        var creatureTilePos = GetTilePosition(selectedCreature.transform.position);

        // If this placing was outside this grid, do nothing
        if (!IsPositionInGrid(creatureTilePos))
            return;
        
        int i = creatureTilePos.y, j = creatureTilePos.x;
        
        var creature = selectedCreature.GetComponent<Creature>();
        byte width = creature.width, height = creature.height;

        Creature init = null;
        if (placed)
            init = creature;

        grid[i, j] = init;
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                occupances[y + i, -x + j] = placed;

        if (isInventory)
            if (placed) InventoryManager.Instance.AddToInventory(creature);
            else InventoryManager.Instance.RemoveFromInventory(creature);
        else if (isPlayerBoard)
            if (placed) BoardManager.Instance.AddToBoardManager(creature);
            else BoardManager.Instance.RemoveFromBoardManager(creature);
        else if (isShop)
        {
            var price = InventoryManager.Instance.GetPrice(creature);
            if(placed) InventoryManager.Instance.AddMoney(price/2);
            else  InventoryManager.Instance.RemoveMoney(price);
        }
    }
}
