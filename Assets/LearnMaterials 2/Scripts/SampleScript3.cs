using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Создаёт копии префаба вдоль направления forward и плавно поворачивает родительский объект.
/// </summary>
public class SampleScript3 : SampleScript
{
    [Header("Настройки копирования")]
    [Tooltip("Префаб для создания копий")]
    [SerializeField] private GameObject prefabToCopy;

    [Tooltip("Количество создаваемых копий")]
    [SerializeField, Min(1)] private int numberOfCopies = 5;

    [Tooltip("Расстояние между копиями")]
    [SerializeField, Min(0.1f)] private float stepDistance = 2f;

    [Tooltip("Дополнительное смещение для каждой копии")]
    [SerializeField] private Vector3 copyOffset = Vector3.zero;

    [Header("Настройки поворота")]
    [Tooltip("Длительность поворота в секундах")]
    [SerializeField, Min(0.1f)] private float rotationDuration = 9f;

    [Tooltip("Ось вращения")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Tooltip("Угол поворота в градусах")]
    [SerializeField] private float rotationAngle = 90f;

    [Header("Дополнительные настройки")]
    [Tooltip("Если включено, копии станут дочерними объектами и будут вращаться вместе с родителем")]
    [SerializeField] private bool attachCopiesToParent = false;

    private readonly List<GameObject> createdCopies = new List<GameObject>();
    private bool isRotating = false;
    private float currentRotationTime = 0f;
    private Quaternion startRotation;
    private Quaternion targetRotation;

    private void Update()
    {
        if (!isRotating) return;

        currentRotationTime += Time.deltaTime;

        if (currentRotationTime >= rotationDuration)
        {
            transform.rotation = targetRotation;
            isRotating = false;
            Debug.Log($"{name}: Поворот завершён");
        }
        else
        {
            float t = currentRotationTime / rotationDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
        }
    }

    [ContextMenu("Активировать")]
    public override void Use()
    {
        Debug.Log($"=== {nameof(SampleScript3)}.Use() запущен на {name} ===");
        CreateCopies();
        StartSmoothRotation();
    }

    private void CreateCopies()
    {
        if (prefabToCopy == null)
        {
            Debug.LogError($"{nameof(SampleScript3)} на {name}: Префаб не назначен!");
            return;
        }

        Debug.Log($"Создаю {numberOfCopies} копий {prefabToCopy.name} с шагом {stepDistance}");

        for (int i = 0; i < numberOfCopies; i++)
        {
            Vector3 spawnPosition = transform.position + (transform.forward * (i + 1) * stepDistance) + copyOffset;

            Transform copyParent = attachCopiesToParent ? transform : null;
            GameObject newCopy = Instantiate(prefabToCopy, spawnPosition, prefabToCopy.transform.rotation, copyParent);

            newCopy.name = $"{prefabToCopy.name}_Copy_{i + 1}";
            createdCopies.Add(newCopy);

            Debug.Log($"Создана копия {i + 1} на позиции {spawnPosition}");
        }

        Debug.Log($"Создано {numberOfCopies} копий");
    }

    private void StartSmoothRotation()
    {
        startRotation = transform.rotation;
        targetRotation = startRotation * Quaternion.AngleAxis(rotationAngle, rotationAxis);

        currentRotationTime = 0f;
        isRotating = true;

        string childInfo = attachCopiesToParent ? " (копии вращаются вместе)" : " (копии остаются на месте)";
        Debug.Log($"Начинаю поворот на {rotationAngle}° вокруг оси {rotationAxis} за {rotationDuration} сек.{childInfo}");
    }
}
