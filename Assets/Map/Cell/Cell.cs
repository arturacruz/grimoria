using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    Monster,
    Shop
}

public class Cell : MonoBehaviour
{
    public readonly List<Cell> Connections = new();
    public CellType Type = CellType.Monster;
}
