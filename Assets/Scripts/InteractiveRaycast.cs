using UnityEngine;

[DisallowMultipleComponent]
public class InteractiveRaycast : MonoBehaviour
{
    private const int MaxRaycastHits = 32;

    [Header("Spawn")]
    [Tooltip("Префаб куба со скриптом InteractiveBox.")]
    public GameObject prefab;

    [Tooltip("Camera used for click raycasts. If empty, Camera.main is used.")]
    [SerializeField] private Camera raycastCamera;

    [Tooltip("Unity Tag для плоскости спавна (Project Settings → Tags).")]
    [SerializeField] private string interactivePlaneTag = "InteractivePlane";

    [Tooltip("Смещение вдоль нормали, чтобы объект не врезался в поверхность.")]
    [SerializeField, Min(0f)] private float surfacePadding = 0.01f;

    [Header("Raycast")]
    [SerializeField] private LayerMask clickMask = Physics.DefaultRaycastLayers;

    [SerializeField, Min(0f)] private float maxDistance = 1000f;

    private InteractiveBox _selectedBox;

    private readonly RaycastHit[] _raycastHits = new RaycastHit[MaxRaycastHits];

    private void Awake()
    {
        if (raycastCamera == null)
            raycastCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleLeftClick();

        if (Input.GetMouseButtonDown(1))
            HandleRightClick();

        if (!_selectedBox)
            _selectedBox = null;
    }

    private void HandleLeftClick()
    {
        if (!TryGetHit(out RaycastHit hit))
            return;

        InteractiveBox hitBox = hit.collider.GetComponentInParent<InteractiveBox>();
        if (hitBox != null)
        {
            if (_selectedBox == null)
            {
                _selectedBox = hitBox;
                return;
            }

            if (_selectedBox != hitBox)
            {
                _selectedBox.AddNext(hitBox);
                _selectedBox = null;
            }

            return;
        }

        if (string.IsNullOrEmpty(interactivePlaneTag) || !hit.collider.CompareTag(interactivePlaneTag))
            return;

        SpawnPrefabOnSurface(hit);
        _selectedBox = null;
    }

    private void HandleRightClick()
    {
        if (!TryGetHit(out RaycastHit hit))
            return;

        InteractiveBox hitBox = hit.collider.GetComponentInParent<InteractiveBox>();
        if (hitBox == null)
            return;

        if (_selectedBox == hitBox)
            _selectedBox = null;

        Destroy(hitBox.gameObject);
    }

    private bool TryGetHit(out RaycastHit hit)
    {
        hit = default;

        if (raycastCamera == null)
            return false;

        Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
        int count = Physics.RaycastNonAlloc(
            ray,
            _raycastHits,
            maxDistance,
            clickMask,
            QueryTriggerInteraction.Ignore);

        if (count <= 0)
            return false;

        int best = 0;
        for (int i = 1; i < count; i++)
        {
            if (_raycastHits[i].distance < _raycastHits[best].distance)
                best = i;
        }

        hit = _raycastHits[best];
        return true;
    }

    private void SpawnPrefabOnSurface(RaycastHit hit)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"{nameof(InteractiveRaycast)}: prefab is not assigned.", this);
            return;
        }

        Vector3 n = hit.normal.normalized;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, n) * prefab.transform.rotation;
        GameObject spawned = Instantiate(prefab, hit.point, rotation);

        if (spawned.GetComponentInChildren<InteractiveBox>(true) == null)
        {
            Debug.LogWarning(
                $"{nameof(InteractiveRaycast)}: префаб для спавна должен содержать {nameof(InteractiveBox)} (по ТЗ это куб со скриптом интерактивного куба, без {nameof(ObstacleItem)}).",
                this);
            Destroy(spawned);
            return;
        }

        foreach (var oi in spawned.GetComponentsInChildren<ObstacleItem>(true))
            Destroy(oi);

        float offset = ComputeSurfaceOffset(spawned, n) + surfacePadding;
        spawned.transform.position = hit.point + n * offset;
    }

    private static float ComputeSurfaceOffset(GameObject instance, Vector3 normal)
    {
        if (!TryGetWorldBounds(instance, out Bounds bounds))
            return 0.5f;

        Vector3 n = normal.normalized;
        Vector3 e = bounds.extents;

        float support = Mathf.Abs(n.x) * e.x + Mathf.Abs(n.y) * e.y + Mathf.Abs(n.z) * e.z;

        float minAlongNormal = Vector3.Dot(n, bounds.center) - support;
        return Mathf.Max(0f, Vector3.Dot(n, instance.transform.position) - minAlongNormal);
    }

    private static bool TryGetWorldBounds(GameObject obj, out Bounds bounds)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        if (colliders.Length > 0)
        {
            bounds = colliders[0].bounds;
            for (int i = 1; i < colliders.Length; i++)
                bounds.Encapsulate(colliders[i].bounds);
            return true;
        }

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);
            return true;
        }

        bounds = default;
        return false;
    }
}
