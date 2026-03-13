using UnityEngine;
using System.Collections.Generic;

public class SampleScriptsController : MonoBehaviour
{
    [Header("Ссылки на скрипты")]
    [SerializeField] private List<SampleScript> sampleScripts = new List<SampleScript>();
    
    [Header("Тестирование")]
    [SerializeField] private bool testUseAll;

    private void Awake()
    {
        if (sampleScripts.Count == 0)
        {
            FindAllSampleScripts();
        }
    }
    
    private void OnValidate()
    {
        if (testUseAll)
        {
            testUseAll = false;
            if (Application.isPlaying)
            {
                UseAll();
            }
            else
            {
                Debug.LogWarning("Тестирование доступно только в Play Mode!");
            }
        }
    }

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
                Debug.LogError($"Элемент {i} равен null!");
            }
        }
        
        Debug.Log("=== UseAll() завершен ===");
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