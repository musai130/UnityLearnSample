using System.Collections;
using UnityEngine;

public class Rotater : SampleScript
{
    [Tooltip("Скорость вращения (чем больше — тем быстрее)")]
    [SerializeField, Min(0.1f)] private float speed = 2f;

    [Tooltip("Угол поворота вокруг осей X, Y, Z в градусах")]
    [SerializeField] private Vector3 targetAngles;

    private void Start()
    {
        gameObject.isStatic = false;
        Use();
    }

    public override void Use()
    {
        StopAllCoroutines();
        StartCoroutine(RotateToTarget());
    }

    private IEnumerator RotateToTarget()
    {
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(targetAngles);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, Mathf.Clamp01(t));
            yield return null;
        }

        transform.rotation = targetRot;
    }
}
