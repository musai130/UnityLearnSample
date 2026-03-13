using UnityEngine;
using System.Collections.Generic;

public class SampleScript3 : SampleScript
{
    [Header("Настройки копирования")]
    [SerializeField] private GameObject prefabToCopy;        
    [SerializeField] private int numberOfCopies = 5;            
    [SerializeField] private float stepDistance = 2f;           
    [SerializeField] private Vector3 copyOffset = Vector3.zero;
    
    [Header("Настройки поворота")]
    [SerializeField] private float rotationDuration = 9f;    
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationAngle = 90f;        
    
    [Header("Дополнительные настройки")]
    [SerializeField] private bool rotateParentOnly = true;  
    
    [Header("Состояние")]
    [SerializeField] private bool isRotating = false;
    [SerializeField] private float currentRotationTime = 0f;
    [SerializeField] private Quaternion startRotation;
    [SerializeField] private Quaternion targetRotation;

    private readonly List<GameObject> createdCopies = new List<GameObject>();

    private void Update()
    {
        if (isRotating)
        {
            currentRotationTime += Time.deltaTime;
            
            if (currentRotationTime >= rotationDuration)
            {
                transform.rotation = targetRotation;
                isRotating = false;
                Debug.Log($"{name}: Поворот завершен");
            }
            else
            {
    
                float t = currentRotationTime / rotationDuration;

                t = Mathf.SmoothStep(0, 1, t);
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            }
        }
    }

    public override void Use()
    {
        Debug.Log($"=== SampleScript3.Use() запущен на {name} ===");
        
 
        CreateCopies();

        StartSmoothRotation();
    }

    private void CreateCopies()
    {
        if (prefabToCopy == null)
        {
            Debug.LogError($"SampleScript3 на {name}: Префаб не назначен!");
            return;
        }
        

        
        Debug.Log($"Создаю {numberOfCopies} копий {prefabToCopy.name} с шагом {stepDistance}");
        
 
        for (int i = 0; i < numberOfCopies; i++)
        {

            Vector3 spawnPosition = transform.position + (transform.forward * (i + 1) * stepDistance) + copyOffset;
            
   
            GameObject newCopy = Instantiate(prefabToCopy, spawnPosition, prefabToCopy.transform.rotation);
            

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
        
        Debug.Log($"Начинаю поворот на {rotationAngle}° вокруг оси {rotationAxis} за {rotationDuration} сек.");
    
        if (!rotateParentOnly)
        {
      
            Debug.Log("Поворачиваются все дочерние объекты");
        }
    }
    
}