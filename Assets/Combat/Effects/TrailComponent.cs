using System;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailComponent : MonoBehaviour
{
    public Creature target;
    public uint damage;
    public Element element;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 1500f;
    [SerializeField] private TrailRenderer trail;

    [Header("Visual")]
    [SerializeField] private Transform core;
    [SerializeField] private float rotateSpeed = 360f;
    [SerializeField] private float pulseSpeed = 8f;
    [SerializeField] private float pulseAmount = 0.12f;
    [SerializeField] private float trailIntensity = 1.5f;

    private Vector3 coreBaseScale;

    private void Awake()
    {
        if (trail == null)
            trail = GetComponent<TrailRenderer>();

        if (core != null)
            coreBaseScale = core.localScale;
    }

    private void Start()
    {
        ApplyColor();
    }

    private void Update()
    {
        if (core == null)
            return;

        core.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        core.localScale = coreBaseScale * pulse;
    }

    private void ApplyColor()
    {
        if (trail == null)
            return;

        Color c = GetElementColor(element);

        c *= trailIntensity;

        trail.startColor = c;
        trail.endColor = new Color(c.r, c.g, c.b, 0f);

        Gradient g = new Gradient();
        g.SetKeys(
            new[]
            {
                new GradientColorKey(c, 0f),
                new GradientColorKey(c, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trail.colorGradient = g;
    }

    private Color GetElementColor(Element e)
    {
        if (e == null)
            return Color.white;

        return e.GetType().Name switch
        {
            "Blaze" => new Color(1.20f, 0.55f, 0.15f),
            "Blood" => new Color(1.00f, 0.15f, 0.22f),
            "Death" => new Color(1.10f, 1.10f, 1.20f),
            "Plague" => new Color(0.25f, 1.00f, 0.35f),
            "Shadow" => new Color(0.65f, 0.40f, 1.00f),
            "Void" => new Color(1.00f, 0.30f, 1.30f),
            _ => Color.white
        };
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (rb == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 dir = target.transform.position - transform.position;
        rb.linearVelocity = dir.normalized * speed * Time.fixedDeltaTime;

        if (dir.sqrMagnitude < 0.16f)
        {
            target.TakeDamage(damage);
            target = null;
            Destroy(gameObject);
        }
    }
}
