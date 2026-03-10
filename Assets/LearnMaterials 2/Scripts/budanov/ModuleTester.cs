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
}
