using System;
using UnityEngine;

public class TrailComponent : MonoBehaviour
{
    public Creature target;
    public uint damage;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 1500f;

    private void FixedUpdate()
    {
        if (target == null)
            return;

        Vector2 dir = target.transform.position - transform.position;
        rb.linearVelocity = dir.normalized * speed * Time.fixedDeltaTime;

        if (Math.Abs(dir.x) < 0.4f)
        {
            target.TakeDamage(damage);
            target = null;
            Destroy(gameObject);
        }
        
    }
}
