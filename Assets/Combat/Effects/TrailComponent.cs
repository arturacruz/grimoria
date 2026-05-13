using System;
using UnityEngine;

public class TrailComponent : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 1000f;

    private void FixedUpdate()
    {
        if (target == null)
            return;

        Vector2 dir = target.position - transform.position;
        rb.linearVelocity = dir.normalized * speed * Time.fixedDeltaTime;
        
        if (Math.Abs(dir.x) < 0.4f)
            Destroy(gameObject);
        
    }
}
