using System.Collections;
using UnityEngine;

/// <summary>
/// Плавно изменяет масштаб объекта между текущим и целевым значением.
/// Переключает направление при каждом вызове.
/// </summary>
[HelpURL("https://docs.google.com/document/d/1rdTEVSrCcYOjqTJcFCHj46RvnbdJhmQUb3gHMDhVftI/edit?usp=sharing")]
public class ScalerModule : SampleScript
{
    [Header("Настройки масштабирования")]
    [Tooltip("Целевой масштаб объекта")]
    [SerializeField] private Vector3 targetScale = new Vector3(2f, 2f, 2f);

    [Tooltip("Скорость изменения масштаба")]
    [SerializeField, Min(0.1f)] private float changeSpeed = 2f;

    private Vector3 defaultScale;
    private Transform myTransform;
    private bool toDefault;

    private void Awake()
    {
        myTransform = transform;
        defaultScale = myTransform.localScale;
        toDefault = false;
    }

    [ContextMenu("Активировать масштабирование")]
    public override void Use()
    {
        Vector3 target = toDefault ? defaultScale : targetScale;
        StopAllCoroutines();
        StartCoroutine(ScaleCoroutine(target));
        toDefault = !toDefault;
    }

    public void ReturnToDefaultState()
    {
        toDefault = true;
        Use();
    }

    private IEnumerator ScaleCoroutine(Vector3 target)
    {
        Vector3 start = myTransform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * changeSpeed;
            myTransform.localScale = Vector3.Lerp(start, target, Mathf.Clamp01(t));
            yield return null;
        }

        myTransform.localScale = target;
    }
}
