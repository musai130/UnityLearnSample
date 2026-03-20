using System.Collections;
using UnityEngine;

/// <summary>
/// Плавно изменяет прозрачность материала объекта.
/// Работает с Standard Shader (Built-in RP). Для URP/HDRP требуется другой подход.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class TransparentModule : SampleScript
{
    [Header("Настройки прозрачности")]
    [Tooltip("Скорость изменения прозрачности")]
    [SerializeField, Min(0.1f)] private float changeSpeed = 2f;

    [Tooltip("Начальное значение альфа-канала")]
    [SerializeField, Range(0f, 1f)] private float alphaFrom = 1f;

    [Tooltip("Целевое значение альфа-канала")]
    [SerializeField, Range(0f, 1f)] private float alphaTo = 0f;

    private Material mat;
    private bool useAlphaFrom;
    private bool isInitialized;

    private void Awake()
    {
        InitializeMaterial();
    }

    private void InitializeMaterial()
    {
        if (isInitialized) return;

        var renderer = GetComponent<Renderer>();
        mat = renderer.material;
        useAlphaFrom = false;

        if (!SetupTransparencyMode())
        {
            Debug.LogWarning($"{nameof(TransparentModule)} на {name}: Материал не поддерживает Standard Shader. " +
                           "Для URP/HDRP используйте материал с включённой прозрачностью.");
        }

        isInitialized = true;
    }

    private bool SetupTransparencyMode()
    {
        if (!mat.HasProperty("_Mode"))
        {
            return false;
        }

        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        return true;
    }

    [ContextMenu("Активировать прозрачность")]
    public void ActivateModule()
    {
        if (!isInitialized)
        {
            InitializeMaterial();
        }

        float target = useAlphaFrom ? alphaFrom : alphaTo;
        StopAllCoroutines();
        StartCoroutine(ChangeTransparencyCoroutine(new Color(mat.color.r, mat.color.g, mat.color.b, target)));
        useAlphaFrom = !useAlphaFrom;
    }

    public override void Use()
    {
        ActivateModule();
    }

    private IEnumerator ChangeTransparencyCoroutine(Color target)
    {
        Color start = mat.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * changeSpeed;
            mat.color = Color.Lerp(start, target, Mathf.Clamp01(t));
            yield return null;
        }

        mat.color = target;
    }

    private void OnDestroy()
    {
        if (mat != null)
        {
            Destroy(mat);
        }
    }
}
