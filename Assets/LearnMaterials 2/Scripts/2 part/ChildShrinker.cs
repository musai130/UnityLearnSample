using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildShrinker : SampleScript
{
    [Tooltip("Transform, чьи дочерние объекты будут удалены")]
    [SerializeField] private Transform target;

    [Tooltip("Длительность анимации сжатия в секундах")]
    [SerializeField, Min(0.01f)] private float shrinkDuration = 0.5f;

    public override void Use()
    {
        if (target == null)
        {
            Debug.LogWarning("ChildShrinker: target не назначен.");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(ShrinkAndDestroyChildren());
    }

    [ContextMenu("Активировать сжатие детей")]
    public void ActivateModule()
    {
        Use();
    }

    private IEnumerator ShrinkAndDestroyChildren()
    {
        List<Transform> children = new List<Transform>();
        List<Vector3> startScales = new List<Vector3>();

        foreach (Transform child in target)
        {
            children.Add(child);
            startScales.Add(child.localScale);
        }

        // Если у target нет детей, работаем с самим target
        if (children.Count == 0)
        {
            children.Add(target);
            startScales.Add(target.localScale);
        }

        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shrinkDuration);
            float smoothT = 1f - (1f - t) * (1f - t); // Ease-in для плавного замедления в конце

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] != null)
                {
                    children[i].localScale = Vector3.Lerp(startScales[i], Vector3.zero, smoothT);
                }
            }

            yield return null;
        }

        foreach (Transform child in children)
        {
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
