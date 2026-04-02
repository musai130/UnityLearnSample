using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class ObstacleItem : MonoBehaviour
{
    [Header("State")]
    [Range(0f, 1f)] public float currentValue = 1f;

    [Header("Events")]
    [Tooltip("Вызывается один раз при достижении currentValue нуля, перед уничтожением объекта.")]
    public UnityEvent onDestroyObstacle;

    [Tooltip("Необязательно: в Awake их Use() подпишется на onDestroyObstacle (надёжно для автосборки; дублирует ручной список в UnityEvent).")]
    [SerializeField] private SampleScript[] preRegisteredDestroyEffects;

    [Header("Visuals")]
    [Tooltip("Renderer to tint. If empty, the first Renderer in children will be used.")]
    [SerializeField] private Renderer targetRenderer;

    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private MaterialPropertyBlock _mpb;
    private bool _destroyed;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        _mpb ??= new MaterialPropertyBlock();
        ApplyVisual();

        if (preRegisteredDestroyEffects != null && preRegisteredDestroyEffects.Length > 0)
        {
            onDestroyObstacle ??= new UnityEvent();
            foreach (var s in preRegisteredDestroyEffects)
            {
                if (s != null)
                    onDestroyObstacle.AddListener(s.Use);
            }
        }
    }

    private void OnValidate()
    {
        currentValue = Mathf.Clamp01(currentValue);
        if (!Application.isPlaying)
        {
            if (targetRenderer == null)
                targetRenderer = GetComponentInChildren<Renderer>();
            _mpb ??= new MaterialPropertyBlock();
            ApplyVisual();
        }
    }

    public void GetDamage(float value)
    {
        if (_destroyed)
            return;

        if (value <= 0f)
            return;

        currentValue = Mathf.Clamp01(currentValue - value);
        ApplyVisual();

        if (currentValue <= 0f)
        {
            _destroyed = true;
            onDestroyObstacle?.Invoke();
            Destroy(gameObject);
        }
    }

    private void ApplyVisual()
    {
        if (targetRenderer == null)
            return;

        Color c = Color.Lerp(Color.red, Color.white, currentValue);

        targetRenderer.GetPropertyBlock(_mpb);

        _mpb.SetColor(ColorId, c);
        _mpb.SetColor(BaseColorId, c);

        targetRenderer.SetPropertyBlock(_mpb);
    }
}

