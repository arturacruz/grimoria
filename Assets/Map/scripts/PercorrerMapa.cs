using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PercorrerMapa : MonoBehaviour
{
    private float scrollSpeed = 0.7f;
    private float minY;
    private float maxY;
    public MapGenerator map;

    void Start()
    {
        maxY = map.GetRoomDrawPosition(0, map.floors - 1).y - 3f;
        minY = map.GetRoomDrawPosition(0, 0).y + 3f;

        Vector3 pos = transform.position;
        
        if (MapGenerator.Player != null){
            pos.y = MapGenerator.Player.transform.position.y;
        }

        else
        {
            pos.y = minY;
        }
        transform.position = pos;
    }
    void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        Vector3 pos = transform.position;
        pos.y += scroll * scrollSpeed;

        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}
