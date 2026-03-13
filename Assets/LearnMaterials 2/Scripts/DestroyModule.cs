using System.Collections;
using UnityEngine;

[HelpURL("https://docs.google.com/document/d/1RMamVxE-yUpSfsPD_dEa4-Ak1qu6NTo83qY1O4XLxUY/edit?usp=sharing")]
public class DestroyModule : MonoBehaviour
{
    [Header("Настройки уничтожения")]

    [SerializeField]
    [Range(0.1f, 5f)]
    [Tooltip("Задержка между уничтожением объектов (в секундах)")]
    private float destroyDelay = 1f;

    [SerializeField]
    [Min(0)]
    [Tooltip("Минимальное количество объектов, которое должно остаться")]
    private int minimalDestroyingObjectsCount = 0;

    [SerializeField]
    [Tooltip("Уничтожить сам объект после удаления всех детей")]
    private bool destroySelfAfterCompletion = true;

    private Transform myTransform;

    private void Awake()
    {
        myTransform = transform;
    }

    public void ActivateModule()
    {
        // Проверка, есть ли что удалять
        if (myTransform.childCount <= minimalDestroyingObjectsCount)
        {
            Debug.LogWarning("Недостаточно объектов для уничтожения");
            return;
        }

        StartCoroutine(DestroyRandomChildObjectCoroutine());
    }

    private IEnumerator DestroyRandomChildObjectCoroutine()
    {
        while (myTransform.childCount > minimalDestroyingObjectsCount)
        {
            // Случайный индекс от 0 до количества детей
            int index = Random.Range(0, myTransform.childCount);

            // Уничтожаем случайного ребенка
            Destroy(myTransform.GetChild(index).gameObject);

            // Ждем указанное время
            yield return new WaitForSeconds(destroyDelay);
        }

        // Уничтожаем сам объект, если нужно
        if (destroySelfAfterCompletion)
        {
            Destroy(gameObject, Time.deltaTime);
        }
    }

    // Визуализация в редакторе
    private void OnDrawGizmosSelected()
    {
        if (myTransform == null) myTransform = transform;

        // Показываем количество детей
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up,
            $"Детей: {myTransform.childCount}");
#endif
    }

    // 👇 ВОТ ЭТА КНОПКА! 👇
    [ContextMenu("Активировать уничтожение")]
    public void TestActivate()
    {
        ActivateModule();
    }
}