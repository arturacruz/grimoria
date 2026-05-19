using System;
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

        gridComponent.grid[pos.y, pos.x] = null;
        for (var y = 0; y < creature.height; y++)
            for (var x = 0; x < creature.width; x++)
                gridComponent.occupances[pos.y + y, pos.x + x] = false;
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
        // Offsets so that, if the creature can't find a target at the same line,
        // it starts alternating between the above and lower lines, until it finds an enemy
        var offsets = new int[height * 2];
        var negate = true;
        
        for (var i = 0; i < offsets.Length; i++)
        {
            if (negate) offsets[i] = -i;
            else offsets[i] = i;
            negate = !negate;
        }
        
        Debug.Log($"offset: {string.Join(", ", offsets)}");

        var newY = y;

        foreach (var offset in offsets)
        {
            newY += offset;
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
        // Offsets so that, if the creature can't find a target at the same line,
        // it starts alternating between the above and lower lines, until it finds an enemy
        var offsets = new int[height * 2];
        var negate = true;
        
        for (var i = 0; i < offsets.Length; i++)
        {
            if (negate) offsets[i] = -i;
            else offsets[i] = i;
            negate = !negate;
        }
        
        var newY = y;

        foreach (var offset in offsets)
        {
            newY += offset;
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