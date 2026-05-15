using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomData
{
    public int x;
    public int y;

    public Vector2 worldPosition;

    public CategoriaCasa tipo;

    public List<Vector2Int> connections =
        new List<Vector2Int>();
}