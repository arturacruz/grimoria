using System.Collections.Generic;
using UnityEngine;

public static class MapData
{
    public static List<RoomData> rooms =
        new List<RoomData>();

    public static int floors;
    public static int columns;

    public static Vector2Int currentRoom;

    public static bool mapGenerated = false;
}