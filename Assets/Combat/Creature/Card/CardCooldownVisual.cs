using UnityEngine;

[RequireComponent(typeof(Creature))]
public class CardCooldownBloodVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer texture;
    [SerializeField] private SpriteRenderer cooldownBlood;
    [SerializeField] private SpriteRenderer border;
    [SerializeField] private SpriteRenderer borderGlow;

    [Header("Blood Shader")]
    [SerializeField] private string heightProperty = "_Height";
    [SerializeField] private string colorProperty = "_Color";
    [SerializeField] private string bubbleColorProperty = "_BubbleColor";

    [Header("Texture")]
    [SerializeField] private float textureAlpha = 1f;

    [Header("Border")]
    [SerializeField] private float borderMinAlpha = 0.15f;
    [SerializeField] private float borderMaxAlpha = 1f;

    [Header("Glow")]
    [SerializeField] private float glowMinAlpha = 0f;
    [SerializeField] private float glowMaxAlpha = 0.9f;
    [SerializeField] private float glowPulseSpeed = 6f;
    [SerializeField] private float glowPulseAmount = 0.03f;

    private Creature creature;
    private MaterialPropertyBlock mpb;

    private Color borderBaseColor;
    private Color glowBaseColor;

    private Vector3 glowBaseScale;

    private static readonly int HeightId = Shader.PropertyToID("_Height");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int BubbleColorId = Shader.PropertyToID("_BubbleColor");

    private void Awake()
    {
        creature = GetComponent<Creature>();
        mpb = new MaterialPropertyBlock();

        if (border != null)
            borderBaseColor = border.color;

        if (borderGlow != null)
        {
            glowBaseColor = borderGlow.color;
            glowBaseScale = borderGlow.transform.localScale;
        }
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

        float eased = Mathf.SmoothStep(0f, 1f, progress);
        UpdateBloodOverlay(eased);
        UpdateBorder(eased);
        UpdateGlow(eased);
    }

    private void UpdateBloodOverlay(float progress)
    {
        if (cooldownBlood == null)
            return;

        Renderer rend = cooldownBlood.GetComponent<Renderer>();
        if (rend == null)
            return;

        rend.GetPropertyBlock(mpb);

        // O shader sobe com o cooldown. Ajuste a lógica se o teu graph usar o contrário.
        mpb.SetFloat(HeightId, progress);

        // Cor principal do sangue
        mpb.SetColor(ColorId, new Color(1f, 0.15f, 0.15f, 1f));

        // Bolhas/energia interna
        mpb.SetColor(BubbleColorId, new Color(0.35f, 0f, 0f, 1f));

        rend.SetPropertyBlock(mpb);
    }

    private void UpdateBorder(float progress)
    {
        if (border == null)
            return;

        Color c = GetElementBorderColor(creature.element);
        c.a = Mathf.Lerp(borderMinAlpha, borderMaxAlpha, progress);
        border.color = c;
    }

    private void UpdateGlow(float progress)
    {
        if (borderGlow == null)
            return;

        Color c = GetElementGlowColor(creature.element);
        c.a = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, progress);
        borderGlow.color = c;

        float pulse = 1f + Mathf.Sin(Time.time * glowPulseSpeed) * glowPulseAmount * progress;
        borderGlow.transform.localScale = glowBaseScale * pulse;
    }

    private Color GetElementBorderColor(Element element)
    {
        if (element == null)
            return new Color(0.7f, 0.2f, 1f);

        return element.GetType().Name switch
        {
            "Blaze" => new Color(1.00f, 0.42f, 0.08f),
            "Blood" => new Color(0.92f, 0.10f, 0.18f),
            "Death" => new Color(0.92f, 0.92f, 0.98f),
            "Plague" => new Color(0.14f, 0.86f, 0.26f),
            "Shadow" => new Color(0.45f, 0.25f, 0.78f),
            "Void" => new Color(0.74f, 0.18f, 1.00f),
            _ => new Color(0.7f, 0.2f, 1f)
        };
    }

    private Color GetElementGlowColor(Element element)
    {
        if (element == null)
            return Color.white;

        return element.GetType().Name switch
        {
            "Blaze" => new Color(1.00f, 0.72f, 0.20f),
            "Blood" => new Color(1.00f, 0.25f, 0.25f),
            "Death" => new Color(1.00f, 1.00f, 1.00f),
            "Plague" => new Color(0.30f, 1.00f, 0.40f),
            "Shadow" => new Color(0.68f, 0.48f, 1.00f),
            "Void" => new Color(0.92f, 0.32f, 1.00f),
            _ => Color.white
        };
    }
}