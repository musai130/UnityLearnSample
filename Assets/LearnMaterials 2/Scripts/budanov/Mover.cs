using UnityEngine;

public class Mover : SampleScript
{
    [SerializeField, Min(0.1f)] private float speed = 1f;
    [SerializeField] private Vector3 targetPosition;
    
    private Vector3 startPosition;
    
    private void Start()
    {
        startPosition = transform.position;
        gameObject.isStatic = false; // Static-объекты не двигаются в рантайме
        Use();
    }
    
    public override void Use()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToTarget());
    }
    
    private System.Collections.IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }
}
