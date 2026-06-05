using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GridComponent gridComponent;

    public bool reflect;
    public byte width => gridComponent.width;
    public byte height => gridComponent.height;
    public uint creaturesAlive;

    public void Awake()
    {
        if (reflect) 
            gridComponent.SetAsEnemy();
    }

    public void StartBattle()
    {
        AuditManagedCreatures();
        creaturesAlive = 0;
        foreach (var c in gridComponent.grid)
        {
            if (c == null) continue;
            creaturesAlive++;
        }
    }
    
    public Creature[,] GetGrid()
    {
        return gridComponent.grid;
    }

    public bool ContainsCreature(Creature creature)
    {
        foreach (var c in GetGrid())
            if (c != null && c.Equals(creature))
                return true;
        return false;
    }

    private void AuditManagedCreatures()
    {
        if (!gridComponent.isPlayerBoard || BoardManager.Instance == null)
            return;

        foreach (var obj in BoardManager.Instance.creatures.ToArray())
        {
            if (obj == null)
                continue;

            var creature = obj.GetComponent<Creature>();
            if (creature == null)
                continue;

            if (ContainsCreature(creature))
                continue;

            gridComponent.TryPlaceExistingCreatureAtWorldPosition(creature, creature.transform.position);
        }
    }

    public Vector2Int GetPositionOfCreature(Creature creature, bool accountReflection)
    {
        var pos = gridComponent.GetTilePosition(creature.transform.position);
        if (reflect && accountReflection)
            pos.x = width - pos.x - creature.width;
        return pos;
    }

    // Returns true if the battle should end
    public bool DestroyCreature(Creature creature)
    {
        var pos = GetPositionOfCreature(creature, false);

        if (pos.y < 0 || pos.y >= height || pos.x < 0 || pos.x >= width)
            return creaturesAlive == 0;

        gridComponent.grid[pos.y, pos.x] = null;
        for (var y = 0; y < creature.height; y++)
            for (var x = 0; x < creature.width; x++)
            {
                var tile = new Vector2Int(pos.x - x, pos.y + y);
                if (IsPositionInGrid(tile))
                    gridComponent.occupances[tile.y, tile.x] = false;
            }

        if (creaturesAlive > 0)
            creaturesAlive--;
        
        if(gridComponent.isPlayerBoard)
            BoardManager.Instance.RemoveFromBoardManager(creature);
        Destroy(creature.gameObject);
            
        return creaturesAlive == 0;
    }

    private Creature GetCreatureAt(int x, int y)
    {
        if (reflect)
            x = width - x - 1;
        
        if (y < 0 || y >= height || x < 0 || x >= width)
            return null;

        return GetGrid()[y, x];
    }

    private bool IsPositionInGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    private IEnumerable<int> GetLineSearchOrder(int originY)
    {
        yield return originY;

        for (var offset = 1; offset < height; offset++)
        {
            yield return originY - offset;
            yield return originY + offset;
        }
    }

    public Creature[] GetAOETargets()
    {
        var creatures = new Creature[creaturesAlive];
        var i = 0;
        foreach (var c in GetGrid())
        {
            if (c != null)
            {
                creatures[i] = c;
                i++;
            }
        }

        return creatures;
    }

    public Creature[] GetMeleeTargetAt(int y)
    {
        foreach (var newY in GetLineSearchOrder(y))
        {
            for (var x = width - 1; x >= 0; x--)
            {
                var creature = GetCreatureAt(x, newY);
                if (creature != null)
                    return new []{ creature };
            }
        }

        return new Creature[] { };
    }
    
    public Creature[] GetFlankTargetAt(int y)
    {
        foreach (var newY in GetLineSearchOrder(y))
        {
            for (var x = 0; x < width; x++)
            {
                var creature = GetCreatureAt(x, newY);
                if (creature != null)
                    return new []{creature};
            }
        }

        return new Creature[]{};
    }




}
