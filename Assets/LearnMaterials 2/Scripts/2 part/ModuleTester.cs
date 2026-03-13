using System.Collections.Generic;
using UnityEngine;

public class ModuleTester : MonoBehaviour
{
    [SerializeField] private TransparentModule transparentModule;
    
    [ContextMenu("Активировать прозрачность")]
    public void TestActivate()
    {
        if (transparentModule != null)
        {
            transparentModule.ActivateModule();
            Debug.Log("Прозрачность активирована!");
        }
        else
        {
            Debug.LogError("Перетащи TransparentModule в поле!");
        }
    }

    [SerializeField] private List<SampleScript> samples = new List<SampleScript>();
    
    [ContextMenu("Запустить все Use()")]
    public void ExecuteAll()
    {
        foreach (var sample in samples)
        {
            sample.Use();
        }
    }
    
    private void OnValidate()
    {
        samples.Clear();
        SampleScript[] found = Object.FindObjectsByType<SampleScript>(FindObjectsSortMode.None);

        samples.AddRange(found);
    }
}
