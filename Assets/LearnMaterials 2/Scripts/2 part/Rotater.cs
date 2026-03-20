using System.Collections;
using UnityEngine;

/// <summary>
/// Плавно поворачивает объект к заданным углам Эйлера.
/// Может работать как с собой, так и с указанным дочерним объектом.
/// </summary>
public class Rotater : SampleScript
{
    public enum RotationMode
    {
        [Tooltip("Локальный поворот к указанным углам (относительно родителя)")]
        LocalAbsolute,
        [Tooltip("Добавить указанные углы к текущему локальному повороту")]
        LocalRelative
    }

    [Header("Цель")]
    [Tooltip("Объект для поворота. Если пусто — поворачивается сам скрипт")]
    [SerializeField] private Transform target;

    [Header("Настройки поворота")]
    [Tooltip("Режим поворота:\n" +
             "- LocalAbsolute: повернуть к указанным локальным углам\n" +
             "- LocalRelative: добавить углы к текущему повороту")]
    [SerializeField] private RotationMode mode = RotationMode.LocalAbsolute;

    [Tooltip("Скорость вращения (чем больше — тем быстрее)")]
    [SerializeField, Min(0.1f)] private float speed = 2f;

    [Tooltip("Угол поворота вокруг осей X, Y, Z в градусах")]
    [SerializeField] private Vector3 targetAngles;

    [Header("Отладка")]
    [Tooltip("Выводить информацию о повороте в консоль")]
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

    [ContextMenu("Активировать поворот")]
    public override void Use()
    {
        StopAllCoroutines();
        StartCoroutine(RotateToTarget());
    }

    private IEnumerator RotateToTarget()
    {
        Quaternion startRot = Target.localRotation;
        Quaternion targetRot;

        switch (mode)
        {
            case RotationMode.LocalAbsolute:
                targetRot = Quaternion.Euler(targetAngles);
                break;

            case RotationMode.LocalRelative:
                targetRot = startRot * Quaternion.Euler(targetAngles);
                break;

            default:
                targetRot = Quaternion.Euler(targetAngles);
                break;
        }

        if (debugLog)
        {
            Debug.Log($"{name}: Поворот [{mode}] объекта '{Target.name}' из {startRot.eulerAngles} в {targetRot.eulerAngles}");
        }

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            Target.localRotation = Quaternion.Slerp(startRot, targetRot, Mathf.Clamp01(t));
            yield return null;
        }

        Target.localRotation = targetRot;

        if (debugLog)
        {
            Debug.Log($"{name}: Поворот завершён. Финальные углы: {Target.localEulerAngles}");
        }
    }
}
