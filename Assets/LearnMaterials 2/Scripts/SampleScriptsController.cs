using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Контроллер для управления всеми SampleScript на сцене.
/// Позволяет вызывать Use() у всех скриптов одновременно.
/// </summary>
public class SampleScriptsController : MonoBehaviour
{
    [Header("Ссылки на скрипты")]
    [Tooltip("Список всех управляемых SampleScript")]
    [SerializeField] private List<SampleScript> sampleScripts = new List<SampleScript>();

    [Header("Настройки")]
    [Tooltip("Автоматически запускать UseAll() при старте сцены")]
    [SerializeField] private bool autoStartOnPlay = true;

    private void Awake()
    {
        if (sampleScripts.Count == 0)
        {
            FindAllSampleScripts();
        }
    }

    private void Start()
    {
        if (autoStartOnPlay)
        {
            UseAll();
        }
    }

    [ContextMenu("Запустить все скрипты")]
    public void UseAll()
    {
        Debug.Log($"=== UseAll() запущен. Скриптов в списке: {sampleScripts.Count} ===");

        for (int i = 0; i < sampleScripts.Count; i++)
        {
            if (sampleScripts[i] != null)
            {
                Debug.Log($"Вызываю Use() у {sampleScripts[i].name} ({sampleScripts[i].GetType().Name})");
                sampleScripts[i].Use();
            }
            else
            {
                Debug.LogError($"{nameof(SampleScriptsController)}: Элемент {i} равен null!");
            }
        }

        Debug.Log("=== UseAll() завершён ===");
    }

    [ContextMenu("Найти все SampleScript")]
    public void FindAllSampleScripts()
    {
        sampleScripts.Clear();

        SampleScript[] foundScripts = GetComponentsInChildren<SampleScript>(true);
        sampleScripts.AddRange(foundScripts);

        Debug.Log($"Найдено {sampleScripts.Count} SampleScript");
    }
}
