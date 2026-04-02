#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Один раз в редакторе: префаб InteractiveBox, сцена с InteractivePlane, препятствиями и отдельными объектами с Use() в OnDestroyObstacle.
/// Меню: Tools → Build Interactive Obstacle Demo (TZ).
/// </summary>
public static class InteractiveObstacleDemoBuilder
{
    private const string PrefabPath = "Assets/Prefabs/InteractiveBox.prefab";
    private const string ScenePath = "Assets/InteractiveObstacleDemo.unity";

    private const string ScalerPrefabPath = "Assets/LearnMaterials 2/Learn prefabs 2/ScalerModule.prefab";
    private const string TransparentPrefabPath = "Assets/LearnMaterials 2/Learn prefabs 2/TransparentModule.prefab";
    private const string DestroyPrefabPath = "Assets/LearnMaterials 2/Learn prefabs 2/DestroyModule.prefab";

    [MenuItem("Tools/Build Interactive Obstacle Demo (TZ)", true)]
    private static bool BuildValidate()
    {
        return !EditorApplication.isPlaying;
    }

    [MenuItem("Tools/Build Interactive Obstacle Demo (TZ)")]
    public static void Build()
    {
        // Работу с ассетами/префабами/сценой нельзя делать синхронно из стека GenericMenu —
        // иначе Unity 2022+ сообщает "Reload Assembly called from managed code directly".
        EditorApplication.delayCall += BuildAfterMenuCloses;
    }

    private static void BuildAfterMenuCloses()
    {
        EditorApplication.delayCall -= BuildAfterMenuCloses;
        try
        {
            BuildCore();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private static void BuildCore()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog(
                "Interactive Obstacle Demo",
                "Сборка сцен EditorSceneManager.NewScene недоступна во время Play Mode. Остановите воспроизведение и повторите.",
                "OK");
            return;
        }

        EnsureTagInteractivePlane();
        EnsureDirectory(Path.GetDirectoryName(PrefabPath));
        var boxPrefab = CreateOrLoadInteractiveBoxPrefab();
        BuildScene(boxPrefab);
        Debug.Log("Interactive Obstacle Demo: готово. Сцена: " + ScenePath + ". В Game View включите Gizmos, чтобы видеть Debug.DrawLine.");
    }

    private static void EnsureDirectory(string dir)
    {
        if (string.IsNullOrEmpty(dir) || Directory.Exists(dir))
            return;
        Directory.CreateDirectory(dir);
    }

    private static void EnsureTagInteractivePlane()
    {
        const string tag = "InteractivePlane";
        var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (assets == null || assets.Length == 0)
            return;

        var so = new SerializedObject(assets[0]);
        var tags = so.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
        {
            if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                return;
        }

        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        so.ApplyModifiedProperties();
    }

    private static GameObject CreateOrLoadInteractiveBoxPrefab()
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (existing != null)
        {
            SanitizeInteractiveBoxPrefabAsset(PrefabPath);
            return AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        }

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "InteractiveBox";
        go.AddComponent<InteractiveBox>();
        PrefabUtility.SaveAsPrefabAsset(go, PrefabPath);
        Object.DestroyImmediate(go);
        return AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
    }

    /// <summary>
    /// Куб для ЛКМ по плоскости — только <see cref="InteractiveBox"/>; <see cref="ObstacleItem"/> на нём по ТЗ не должен быть.
    /// </summary>
    private static void SanitizeInteractiveBoxPrefabAsset(string assetPath)
    {
        using (var scope = new PrefabUtility.EditPrefabContentsScope(assetPath))
        {
            var root = scope.prefabContentsRoot;
            foreach (var oi in root.GetComponentsInChildren<ObstacleItem>(true))
                Object.DestroyImmediate(oi, true);

            if (root.GetComponentInChildren<InteractiveBox>(true) == null)
                root.AddComponent<InteractiveBox>();
        }
    }

    private static void BuildScene(GameObject interactiveBoxPrefab)
    {
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var cam = Camera.main;
        if (cam != null)
        {
            cam.transform.SetPositionAndRotation(new Vector3(0f, 7.5f, -9f), Quaternion.Euler(35f, 0f, 0f));
            var ir = cam.GetComponent<InteractiveRaycast>();
            if (ir == null)
                ir = cam.gameObject.AddComponent<InteractiveRaycast>();
            ir.prefab = interactiveBoxPrefab;
        }

        var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "InteractivePlane";
        plane.tag = "InteractivePlane";
        plane.transform.position = Vector3.zero;
        plane.transform.localScale = new Vector3(2f, 1f, 2f);

        var o1 = CreateObstacle("Obstacle_A", new Vector3(-3.5f, 0.5f, -1f));
        var e1 = InstantiateLearnPrefab(ScalerPrefabPath, new Vector3(-3.5f, 0.5f, 1.5f), "ScalerModule (instance)");
        WireOnDestroy(o1, e1);

        var o2 = CreateObstacle("Obstacle_B", new Vector3(0f, 0.5f, -1f));
        var e2 = InstantiateLearnPrefab(TransparentPrefabPath, new Vector3(0f, 0.5f, 1.5f), "TransparentModule (instance)");
        WireOnDestroy(o2, e2);

        var o3 = CreateObstacle("Obstacle_C", new Vector3(3.5f, 0.5f, -1f));
        var e3 = InstantiateLearnPrefab(DestroyPrefabPath, new Vector3(3.5f, 0.5f, 1.5f), "DestroyModule (instance)");
        WireOnDestroy(o3, e3);

        var scene = SceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(scene, ScenePath);
    }

    private static GameObject CreateObstacle(string name, Vector3 position)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.position = position;
        cube.AddComponent<ObstacleItem>();
        return cube;
    }

    private static GameObject InstantiateLearnPrefab(string assetPath, Vector3 position, string instanceName)
    {
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (asset == null)
        {
            Debug.LogError($"{nameof(InteractiveObstacleDemoBuilder)}: нет префаба {assetPath}");
            return null;
        }

        var go = (GameObject)PrefabUtility.InstantiatePrefab(asset);
        go.name = instanceName;
        go.transform.position = position;
        int ignoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        if (ignoreRaycast >= 0)
            SetLayerRecursively(go, ignoreRaycast);
        return go;
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    private static void WireOnDestroy(GameObject obstacle, GameObject effectInstance)
    {
        if (effectInstance == null)
            return;

        var target = effectInstance.GetComponentInChildren<SampleScript>(true);
        if (target == null)
        {
            Debug.LogError($"{nameof(InteractiveObstacleDemoBuilder)}: на «{effectInstance.name}» нет SampleScript с Use().");
            return;
        }

        var oi = obstacle.GetComponent<ObstacleItem>();
        // Ссылки в preRegisteredDestroyEffects → в Awake подписка Use() на UnityEvent (стабильнее чем только UnityEventTools).
        var so = new SerializedObject(oi);
        var arr = so.FindProperty("preRegisteredDestroyEffects");
        arr.ClearArray();
        arr.InsertArrayElementAtIndex(0);
        arr.GetArrayElementAtIndex(0).objectReferenceValue = target;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(oi);
    }
}
#endif
