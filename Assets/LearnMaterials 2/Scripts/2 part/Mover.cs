using System.Collections;
using UnityEngine;

/// <summary>
/// Плавно перемещает объект к заданной позиции.
/// Может работать как с собой, так и с указанным дочерним объектом.
/// </summary>
public class Mover : SampleScript
{
    public enum PositionMode
    {
        [Tooltip("Локальные координаты относительно родителя")]
        LocalPosition,
        [Tooltip("Смещение относительно текущей позиции объекта")]
        Offset
    }

    [Header("Цель")]
    [Tooltip("Объект для перемещения. Если пусто — перемещается сам скрипт")]
    [SerializeField] private Transform target;

    [Header("Настройки перемещения")]
    [Tooltip("Режим позиционирования:\n" +
             "- LocalPosition: локальные координаты (относительно родителя)\n" +
             "- Offset: смещение от текущей позиции")]
    [SerializeField] private PositionMode mode = PositionMode.LocalPosition;

    [Tooltip("Скорость перемещения (единиц в секунду)")]
    [SerializeField, Min(0.1f)] private float speed = 1f;

    [Tooltip("Целевая позиция или смещение (в зависимости от режима)")]
    [SerializeField] private Vector3 targetPosition;

    [Header("Отладка")]
    [Tooltip("Выводить информацию о перемещении в консоль")]
    [SerializeField] private bool debugLog = false;

    private Transform Target => target != null ? target : transform;

    private void Awake()
    {
        if (target == null)
        {
            target = transform;
        }
        Target.gameObject.isStatic = false;
    }

    [ContextMenu("Активировать перемещение")]
    public override void Use()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        Vector3 destination;
        Vector3 startPos = Target.localPosition;

        switch (mode)
        {
            case PositionMode.LocalPosition:
                destination = targetPosition;
                break;

            case PositionMode.Offset:
                destination = startPos + targetPosition;
                break;

            default:
                destination = targetPosition;
                break;
        }

        if (debugLog)
        {
            Debug.Log($"{name}: Перемещение [{mode}] объекта '{Target.name}' из {startPos} в {destination} (локальные координаты)");
        }

        while (Vector3.Distance(Target.localPosition, destination) > 0.001f)
        {
            Target.localPosition = Vector3.MoveTowards(Target.localPosition, destination, speed * Time.deltaTime);
            yield return null;
        }

        Target.localPosition = destination;

        if (debugLog)
        {
            Debug.Log($"{name}: Перемещение завершено. Финальная локальная позиция: {Target.localPosition}");
        }
    }

    
}
