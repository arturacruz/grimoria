using System;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GridComponent gridComponent;

    public bool reflect;
    public Creature[,] grid => gridComponent.grid;
    public byte width => gridComponent.width;
    public byte height => gridComponent.height;

    public uint CreaturesAlive
    {
        get
        {
            uint count = 0;
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    if (grid[y, x] != null)
                        count++;
            return count;
        }
    }

    public Vector2Int GetPositionOfCreature(Creature creature)
    {
        var pos = gridComponent.GetTilePosition(creature.transform.position);
        if (reflect)
            pos.x = width - pos.x - 1;
        return pos;
    }

    private Creature GetCreatureAt(int x, int y)
    {
        if (reflect)
            x = width - x - 1;

        return grid[y, x];
    }

    public Creature GetMeleeTargetAt(int y)
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

        foreach (var offset in offsets)
        {
            for (var x = 0; x < width; x++)
            {
                var creature = GetCreatureAt(x, y + offset);
                if (creature != null)
                    return creature;
            }
        }

        return null;
    }




}