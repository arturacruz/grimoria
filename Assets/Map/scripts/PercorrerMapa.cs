using UnityEngine;
using UnityEngine.InputSystem;

public class PercorrerMapa : MonoBehaviour
{
    [SerializeField] private float dragSpeed = 0.025f;
    private float minY;
    private float maxY;
    private Vector2 lastPointerPosition;
    private bool dragging;
    public MapGenerator map;

    void Start()
    {
        if (!ResolveMap())
            return;

        Vector3 pos = transform.position;
        
        if (MapGenerator.Player != null){
            pos.y = MapGenerator.Player.transform.position.y;
        }

        else
        {
            pos.y = minY;
        }
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
    void Update()
    {
        if (!ResolveMap())
            return;

        if (Pointer.current == null)
            return;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            lastPointerPosition = Pointer.current.position.ReadValue();
            dragging = true;
            return;
        }

        if (!Pointer.current.press.isPressed)
        {
            dragging = false;
            return;
        }

        var pointerPosition = Pointer.current.position.ReadValue();
        var movement = dragging ? (lastPointerPosition.y - pointerPosition.y) * dragSpeed : 0f;
        lastPointerPosition = pointerPosition;
        dragging = true;

        Vector3 pos = transform.position;
        pos.y += movement;

        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    private bool ResolveMap()
    {
        if (map == null)
            map = MapGenerator.Instance;

        if (map == null)
            return false;

        maxY = map.GetRoomBasePosition(0, map.floors - 1).y - 3f;
        minY = map.GetRoomBasePosition(0, 0).y + 3f;
        return true;
    }
}
