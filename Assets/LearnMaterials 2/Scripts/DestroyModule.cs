using System.Collections;
using UnityEngine;

/// <summary>
/// Последовательно уничтожает дочерние объекты с заданной задержкой.
/// Опционально уничтожает сам объект после завершения.
/// </summary>
[HelpURL("https://docs.google.com/document/d/1RMamVxE-yUpSfsPD_dEa4-Ak1qu6NTo83qY1O4XLxUY/edit?usp=sharing")]
public class DestroyModule : SampleScript
{
    [Header("Настройки уничтожения")]
    [Tooltip("Задержка между уничтожением объектов (в секундах)")]
    [SerializeField, Range(0.1f, 5f)] private float destroyDelay = 1f;

    [Tooltip("Минимальное количество объектов, которое должно остаться")]
    [SerializeField, Min(0)] private int minimalDestroyingObjectsCount = 0;

    [Tooltip("Уничтожить сам объект после удаления всех детей")]
    [SerializeField] private bool destroySelfAfterCompletion = true;

    private Transform myTransform;

    private void Awake()
    {
        myTransform = transform;
    }

    [ContextMenu("Активировать уничтожение")]
    public override void Use()
    {
        if (myTransform.childCount <= minimalDestroyingObjectsCount)
        {
            Debug.LogWarning($"{nameof(DestroyModule)} на {name}: Недостаточно объектов для уничтожения");
            return;
        }

        StartCoroutine(DestroyRandomChildObjectCoroutine());
    }

    private IEnumerator DestroyRandomChildObjectCoroutine()
    {
        while (myTransform.childCount > minimalDestroyingObjectsCount)
        {
            int index = Random.Range(0, myTransform.childCount);
            Destroy(myTransform.GetChild(index).gameObject);
            yield return new WaitForSeconds(destroyDelay);
        }

        if (destroySelfAfterCompletion)
        {
            Destroy(gameObject, Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (myTransform == null) myTransform = transform;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up, $"Детей: {myTransform.childCount}");
#endif
    }
}
