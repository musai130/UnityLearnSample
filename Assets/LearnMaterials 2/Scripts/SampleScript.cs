using UnityEngine;

/// <summary>
/// Базовый абстрактный класс для всех модулей-примеров.
/// Все наследники должны реализовать метод Use() для активации функциональности.
/// </summary>
public abstract class SampleScript : MonoBehaviour
{
    /// <summary>
    /// Активирует основную функциональность модуля.
    /// </summary>
    public abstract void Use();
}
