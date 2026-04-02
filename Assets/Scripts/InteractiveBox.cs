using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveBox : MonoBehaviour
{
    private const int MaxBeamRaycastHits = 24;

    [Tooltip("Единственный следующий InteractiveBox в цепочке или отсутствует.")]
    public InteractiveBox next;

    [Header("Beam visualization")]
    [Tooltip("Цвет луча при свободном пути до next (в Game View включите кнопку Gizmos).")]
    [SerializeField] private Color beamColorClear = Color.cyan;

    [Tooltip("Цвет луча, если на пути есть посторонний коллайдер (в т.ч. препятствие).")]
    [SerializeField] private Color beamColorBlocked = Color.red;

    private readonly RaycastHit[] _beamHits = new RaycastHit[MaxBeamRaycastHits];

    public void AddNext(InteractiveBox box)
    {
        if (box == this)
        {
            Debug.LogWarning($"{nameof(InteractiveBox)}.{nameof(AddNext)}: игнорируется ссылка на себя.", this);
            return;
        }

        next = box;
    }

    private void Update()
    {
        if (!TryResolveNext(out InteractiveBox target))
            return;

        Vector3 beamStart = transform.position;
        Vector3 beamEnd = target.transform.position;
        Vector3 toNext = beamEnd - beamStart;
        float distanceToNext = toNext.magnitude;
        if (distanceToNext <= Mathf.Epsilon)
            return;

        Vector3 direction = toNext / distanceToNext;

        bool hitObstacle = TryGetObstacleAlongBeam(
            beamStart,
            direction,
            distanceToNext,
            target,
            out ObstacleItem obstacle,
            out bool lineOfSightClear);

        Debug.DrawLine(beamStart, beamEnd, lineOfSightClear ? beamColorClear : beamColorBlocked, 0f);

        if (hitObstacle && obstacle != null)
            obstacle.GetDamage(Time.deltaTime);
    }

    private bool TryResolveNext(out InteractiveBox target)
    {
        target = null;
        if (next == null)
            return false;

        if (!next)
        {
            next = null;
            return false;
        }

        target = next;
        return true;
    }


    private bool TryGetObstacleAlongBeam(
        Vector3 beamStartWorld,
        Vector3 beamDirectionNormalized,
        float distanceWorldToNext,
        InteractiveBox beamTarget,
        out ObstacleItem obstacle,
        out bool lineOfSightClear)
    {
        obstacle = null;
        lineOfSightClear = false;

        if (distanceWorldToNext <= Mathf.Epsilon)
            return false;

        int count = Physics.RaycastNonAlloc(
            beamStartWorld,
            beamDirectionNormalized,
            _beamHits,
            distanceWorldToNext,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);

        if (count == 0)
        {
            lineOfSightClear = true;
            return false;
        }

        Array.Sort(_beamHits, 0, count, Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));

        lineOfSightClear = true;
        for (int i = 0; i < count; i++)
        {
            Collider c = _beamHits[i].collider;
            if (ShouldIgnoreBeamCollider(c, beamTarget))
                continue;

            lineOfSightClear = false;
            obstacle = c.GetComponentInParent<ObstacleItem>();
            if (obstacle != null)
                return true;
        }

        obstacle = null;
        return false;
    }

    private bool ShouldIgnoreBeamCollider(Collider collider, InteractiveBox beamTarget)
    {
        if (BelongsToTransformHierarchy(collider.transform, transform))
            return true;
        if (beamTarget != null && BelongsToTransformHierarchy(collider.transform, beamTarget.transform))
            return true;
        return false;
    }

    private static bool BelongsToTransformHierarchy(Transform colliderTransform, Transform root)
    {
        if (root == null)
            return false;
        return colliderTransform == root || colliderTransform.IsChildOf(root);
    }
}
