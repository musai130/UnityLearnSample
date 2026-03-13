using UnityEngine;

public class PrefabRepeater : SampleScript
{
    [Tooltip("Префаб для создания копий")]
    [SerializeField] private GameObject prefab;

    [Tooltip("Количество создаваемых экземпляров")]
    [SerializeField, Min(1)] private int count = 5;

    [Tooltip("Шаг (дистанция) между экземплярами")]
    [SerializeField, Min(0.01f)] private float step = 2f;

    [Tooltip("Направление линии (по умолчанию — вперёд по оси объекта)")]
    [SerializeField] private Vector3 direction = Vector3.forward;

    [Tooltip("Родитель для созданных объектов (пусто = корень сцены)")]
    [SerializeField] private Transform parent;

    private void Start()
    {
        Use();
    }

    public override void Use()
    {
        if (prefab == null)
        {
            Debug.LogWarning("PrefabRepeater: префаб не назначен.");
            return;
        }

        Vector3 dir = direction.sqrMagnitude > 0.001f ? direction.normalized : transform.forward;
        Vector3 pos = transform.position;

        for (int i = 0; i < count; i++)
        {
            GameObject instance = Object.Instantiate(prefab, pos, transform.rotation, parent);
            pos += dir * step;
        }
    }
}
