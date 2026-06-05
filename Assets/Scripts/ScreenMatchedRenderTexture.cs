using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenMatchedRenderTexture : MonoBehaviour
{
    private const string SourceTextureName = "FireTexture";
    private static readonly int TextureId = Shader.PropertyToID("_Texture2D");

    private Camera targetCamera;
    private RenderTexture sourceTexture;
    private RenderTexture runtimeTexture;
    private int currentWidth;
    private int currentHeight;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        AttachToDistortionCameras();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AttachToDistortionCameras();
    }

    private static void AttachToDistortionCameras()
    {
        foreach (var camera in FindObjectsOfType<Camera>(true))
        {
            if (camera.targetTexture == null || camera.targetTexture.name != SourceTextureName)
                continue;

            if (camera.GetComponent<ScreenMatchedRenderTexture>() == null)
                camera.gameObject.AddComponent<ScreenMatchedRenderTexture>();
        }
    }

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
        sourceTexture = targetCamera != null ? targetCamera.targetTexture : null;
        RefreshTexture();
    }

    private void OnEnable()
    {
        RefreshTexture();
    }

    private void Update()
    {
        if (Screen.width != currentWidth || Screen.height != currentHeight)
            RefreshTexture();
    }

    private void OnDestroy()
    {
        if (targetCamera != null && targetCamera.targetTexture == runtimeTexture)
            targetCamera.targetTexture = sourceTexture;

        ReleaseRuntimeTexture();
    }

    private void RefreshTexture()
    {
        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();

        if (targetCamera == null)
            return;

        var width = Mathf.Max(1, Screen.width);
        var height = Mathf.Max(1, Screen.height);

        if (runtimeTexture != null && width == currentWidth && height == currentHeight)
            return;

        ReleaseRuntimeTexture();

        currentWidth = width;
        currentHeight = height;
        runtimeTexture = new RenderTexture(currentWidth, currentHeight, 16, RenderTextureFormat.ARGB32)
        {
            name = $"{SourceTextureName}_Runtime",
            antiAliasing = 1,
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            useMipMap = false,
            autoGenerateMips = false
        };
        runtimeTexture.Create();

        targetCamera.targetTexture = runtimeTexture;
        ApplyTextureToMovementMaterials(runtimeTexture);
    }

    private void ReleaseRuntimeTexture()
    {
        if (runtimeTexture == null)
            return;

        runtimeTexture.Release();
        Destroy(runtimeTexture);
        runtimeTexture = null;
    }

    private void ApplyTextureToMovementMaterials(Texture texture)
    {
        Shader.SetGlobalTexture(TextureId, texture);

        foreach (var renderer in FindObjectsOfType<Renderer>(true))
        {
            if (!RendererUsesSourceTexture(renderer))
                continue;

            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.SetTexture(TextureId, texture);
            renderer.SetPropertyBlock(block);
        }
    }

    private bool RendererUsesSourceTexture(Renderer renderer)
    {
        foreach (var material in renderer.sharedMaterials)
        {
            if (material == null || !material.HasProperty(TextureId))
                continue;

            var materialTexture = material.GetTexture(TextureId);
            if (materialTexture == sourceTexture || materialTexture != null && materialTexture.name == SourceTextureName)
                return true;

            if (material.shader != null && material.shader.name.Contains("Movement"))
                return true;
        }

        return false;
    }
}
