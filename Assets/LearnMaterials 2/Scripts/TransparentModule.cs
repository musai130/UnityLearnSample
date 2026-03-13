using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TransparentModule : MonoBehaviour
{
    [Header("Настройки прозрачности")]

    [Tooltip("Скорость изменения прозрачности")]
    [SerializeField, Min(0.1f)]
    private float changeSpeed = 2f;

    [Tooltip("Альфа от")]
    [SerializeField, Range(0f, 1f)]
    private float alphaFrom = 1f;

    [Tooltip("Альфа до")]
    [SerializeField, Range(0f, 1f)]
    private float alphaTo = 0f;

    private Material mat;
    private bool useAlphaFrom;

    private void Start()
    {
        var renderer = GetComponent<Renderer>();
        mat = renderer.material;

        useAlphaFrom = false;

        if (mat.HasProperty("_Mode"))
        {
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }

        ActivateModule();
    }

    public void ActivateModule()
    {
        float target = useAlphaFrom ? alphaFrom : alphaTo;
        StopAllCoroutines();
        StartCoroutine(ChangeTransparencyCoroutine(new Color(mat.color.r, mat.color.g, mat.color.b, target)));
        useAlphaFrom = !useAlphaFrom;
    }

    private IEnumerator ChangeTransparencyCoroutine(Color target)
    {
        Color start = mat.color;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * changeSpeed;
            mat.color = Color.Lerp(start, target, t);
            yield return null;
        }
        mat.color = target;
    }

    private void OnDestroy()
    {
        if (mat != null)
            Destroy(mat);
    }
}