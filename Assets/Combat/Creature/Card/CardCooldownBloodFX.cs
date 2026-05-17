using UnityEngine;

[RequireComponent(typeof(Creature))]
public class CardCooldownBloodFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer cooldownShade;
    [SerializeField] private SpriteRenderer border;
    [SerializeField] private SpriteRenderer borderGlow;

    [Header("Blood Shader Properties")]
    [SerializeField] private string heightProperty = "_Height";
    [SerializeField] private string colorProperty = "_Color";
    [SerializeField] private string bubbleColorProperty = "_BubbleColor";

    [Header("Blood Rise")]
    [SerializeField] private float minVerticalScale = 0.08f;
    [SerializeField] private float maxVerticalScale = 1f;
    [SerializeField] private bool invertProgress = false;

    [Header("Shade Feel")]
    [SerializeField] private float shadeMinAlpha = 0.10f;
    [SerializeField] private float shadeMaxAlpha = 0.70f;

    [Header("Border")]
    [SerializeField] private float borderMinAlpha = 0.08f;
    [SerializeField] private float borderMaxAlpha = 0.95f;

    [Header("Glow")]
    [SerializeField] private float glowMinAlpha = 0f;
    [SerializeField] private float glowMaxAlpha = 0.75f;
    [SerializeField] private float glowPulseSpeed = 6f;
    [SerializeField] private float glowPulseAmount = 0.03f;

    [Header("Blood Shader Colors")]
    [SerializeField] private Color defaultBloodColor = new Color(0.18f, 0.18f, 0.18f, 1f);
    [SerializeField] private Color defaultBubbleColor = new Color(0.05f, 0.05f, 0.05f, 1f);

    private Creature creature;
    private MaterialPropertyBlock block;

    private Vector3 bloodBaseScale;
    private Vector3 bloodBaseLocalPos;
    private Vector3 borderGlowBaseScale;

    private static readonly int HeightId = Shader.PropertyToID("_Height");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int BubbleColorId = Shader.PropertyToID("_BubbleColor");

    private void Awake()
    {
        creature = GetComponent<Creature>();
        block = new MaterialPropertyBlock();

        if (cooldownShade != null)
        {
            bloodBaseScale = cooldownShade.transform.localScale;
            bloodBaseLocalPos = cooldownShade.transform.localPosition;
        }

        if (borderGlow != null)
            borderGlowBaseScale = borderGlow.transform.localScale;
    }

    private void Update()
    {
        if (creature == null || creature.cooldown == null)
            return;

        float progress = 0f;

        if (creature.cooldown.started)
        {
            float duration = Mathf.Max(0.0001f, creature.cooldown.timeSeconds);
            progress = Mathf.Clamp01(creature.cooldown.ElapsedTimeSec() / duration);
        }

        if (invertProgress)
            progress = 1f - progress;

        float eased = Mathf.SmoothStep(0f, 1f, progress);
        float late = Mathf.SmoothStep(0.72f, 1f, progress);

        UpdateBloodShade(eased, late);
        UpdateBorder(eased, late);
        UpdateGlow(eased, late);
    }

    private void UpdateBloodShade(float progress, float late)
    {
        if (cooldownShade == null)
            return;

        var rend = cooldownShade.GetComponent<Renderer>();
        if (rend == null)
            return;

        Color elementShade = GetDarkElementColor(creature.element);
        Color bubble = GetDarkBubbleColor(creature.element);

        rend.GetPropertyBlock(block);
        block.SetFloat(HeightId, progress);
        block.SetColor(ColorId, elementShade);
        block.SetColor(BubbleColorId, bubble);
        rend.SetPropertyBlock(block);

        float yScale = Mathf.Lerp(minVerticalScale, maxVerticalScale, progress);
        float pulse = 1f + Mathf.Sin(Time.time * 4f) * 0.02f * progress;

        cooldownShade.transform.localScale = new Vector3(
            bloodBaseScale.x,
            bloodBaseScale.y * yScale * pulse,
            bloodBaseScale.z
        );

        float offset = (1f - yScale) * 0.5f;
        cooldownShade.transform.localPosition = new Vector3(
            bloodBaseLocalPos.x,
            bloodBaseLocalPos.y - offset,
            bloodBaseLocalPos.z
        );
    }

    private void UpdateBorder(float progress, float late)
    {
        if (border == null)
            return;

        Color c = GetBorderColor(creature.element);

        // mais escuro no começo, mais vivo só perto do fim
        float alpha = Mathf.Lerp(borderMinAlpha, borderMaxAlpha, late);
        c.a = alpha;
        border.color = c;
    }

    private void UpdateGlow(float progress, float late)
    {
        if (borderGlow == null)
            return;

        Color c = GetGlowColor(creature.element);
        c.a = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, late);
        borderGlow.color = c;

        float pulse = 1f + Mathf.Sin(Time.time * glowPulseSpeed) * glowPulseAmount * late;
        borderGlow.transform.localScale = borderGlowBaseScale * pulse;
    }

    private Color GetDarkElementColor(Element element)
    {
        if (element == null)
            return new Color(0.18f, 0.18f, 0.18f, 1f);

        return element.GetType().Name switch
        {
            "Blaze" => new Color(0.35f, 0.14f, 0.05f, 1f),
            "Blood" => new Color(0.28f, 0.05f, 0.08f, 1f),
            "Death" => new Color(0.16f, 0.16f, 0.18f, 1f),
            "Plague" => new Color(0.08f, 0.20f, 0.08f, 1f),
            "Shadow" => new Color(0.11f, 0.07f, 0.22f, 1f),
            "Void" => new Color(0.18f, 0.04f, 0.26f, 1f),
            _ => new Color(0.18f, 0.18f, 0.18f, 1f)
        };
    }

    private Color GetDarkBubbleColor(Element element)
    {
        if (element == null)
            return defaultBubbleColor;

        return element.GetType().Name switch
        {
            "Blaze" => new Color(0.18f, 0.06f, 0.02f, 1f),
            "Blood" => new Color(0.12f, 0.00f, 0.02f, 1f),
            "Death" => new Color(0.08f, 0.08f, 0.10f, 1f),
            "Plague" => new Color(0.03f, 0.10f, 0.03f, 1f),
            "Shadow" => new Color(0.05f, 0.02f, 0.10f, 1f),
            "Void" => new Color(0.10f, 0.02f, 0.14f, 1f),
            _ => defaultBubbleColor
        };
    }

    private Color GetBorderColor(Element element)
    {
        if (element == null)
            return new Color(0.12f, 0.12f, 0.12f, 1f);

        return element.GetType().Name switch
        {
            "Blaze" => new Color(0.85f, 0.35f, 0.08f, 1f),
            "Blood" => new Color(0.75f, 0.08f, 0.14f, 1f),
            "Death" => new Color(0.78f, 0.78f, 0.85f, 1f),
            "Plague" => new Color(0.20f, 0.70f, 0.22f, 1f),
            "Shadow" => new Color(0.45f, 0.28f, 0.72f, 1f),
            "Void" => new Color(0.70f, 0.18f, 0.95f, 1f),
            _ => new Color(0.12f, 0.12f, 0.12f, 1f)
        };
    }

    private Color GetGlowColor(Element element)
    {
        if (element == null)
            return new Color(0.5f, 0.5f, 0.5f, 1f);

        return element.GetType().Name switch
        {
            "Blaze" => new Color(1.00f, 0.65f, 0.20f, 1f),
            "Blood" => new Color(1.00f, 0.20f, 0.25f, 1f),
            "Death" => new Color(1.00f, 1.00f, 1.00f, 1f),
            "Plague" => new Color(0.35f, 1.00f, 0.35f, 1f),
            "Shadow" => new Color(0.72f, 0.50f, 1.00f, 1f),
            "Void" => new Color(0.95f, 0.35f, 1.00f, 1f),
            _ => new Color(0.5f, 0.5f, 0.5f, 1f)
        };
    }
}