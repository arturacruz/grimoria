using System;
using UnityEngine;

public class CameraInventoryMover : MonoBehaviour
{
    [SerializeField] private Vector2 closedPos;
    [SerializeField] private Vector2 openPos;
    [SerializeField] private float speed = 8f;

    private bool open;

    private void Start()
    {
        var creatures = GetComponents<Creature>();
    }

    private void Update()
    {
        Vector2 target = open ? openPos : closedPos;
        Vector3 movement = Vector2.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
        movement.z = -10;
        transform.position = movement;
        
    }

    public void Toggle()
    {
        open = !open;
    }
}