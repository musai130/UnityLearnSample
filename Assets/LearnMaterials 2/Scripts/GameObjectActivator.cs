using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Задаёт указанным объектам значение activeSelf, равное targetState.
/// Переключает состояние при каждом вызове Use().
/// </summary>
[HelpURL("https://docs.google.com/document/d/1GP4_m0MzOF8L5t5pZxLChu3V_TFIq1czi1oJQ2X5kpU/edit?usp=sharing")]
public class GameObjectActivator : SampleScript
{
    [Tooltip("Список объектов и их целевых состояний")]
    [SerializeField] private List<StateContainer> targets;

    [Tooltip("Показывать Gizmo-линии к целевым объектам в редакторе")]
    [SerializeField] private bool debug;

    private void Awake()
    {
        if (targets == null) return;

        foreach (var item in targets)
        {
            if (item.targetGO != null)
            {
                item.defaultValue = item.targetGO.activeSelf;
            }
        }
    }

    [ContextMenu("Активировать переключение")]
    public override void Use()
    {
        SetStateForAll();
    }

    public void ReturnToDefaultState()
    {
        if (targets == null) return;

        foreach (var item in targets)
        {
            if (item.targetGO != null)
            {
                item.targetState = item.defaultValue;
                item.targetGO.SetActive(item.defaultValue);
            }
        }
    }

    private void SetStateForAll()
    {
        if (targets == null) return;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null && targets[i].targetGO != null)
            {
                targets[i].targetGO.SetActive(targets[i].targetState);
                targets[i].targetState = !targets[i].targetState;
            }
            else
            {
                Debug.LogError($"{nameof(GameObjectActivator)}: Элемент {i} равен null. Источник: {gameObject.name}");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!debug || targets == null) return;

        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position, 0.3f);

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null && targets[i].targetGO != null)
            {
                Gizmos.color = targets[i].targetState ? Color.green : Color.red;
                Gizmos.DrawLine(transform.position, targets[i].targetGO.transform.position);
            }
            else
            {
                Debug.LogError($"{nameof(GameObjectActivator)}: Элемент {i} равен null. Источник: {gameObject.name}");
            }
        }
    }
}

/// <summary>
/// Контейнер для хранения ссылки на GameObject и его целевого состояния.
/// </summary>
[System.Serializable]
public class StateContainer
{
    [Tooltip("Объект, которому нужно задать состояние")]
    public GameObject targetGO;

    [Tooltip("Целевое состояние. Если отмечено, объект будет включен")]
    public bool targetState = false;

    [HideInInspector]
    public bool defaultValue;
}
