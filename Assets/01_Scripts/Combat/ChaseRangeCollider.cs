using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ChaseRangeCollider : MonoBehaviour
{
    public Action<Attributes> OnCollisionEnter;
    public Action<Attributes> OnCollisionExit;
    public void SetRadius(float radius)
    {
        if (TryGetComponent<CircleCollider2D>(out var collider))
        {
            collider.radius = radius;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Attributes attributes))
        {
            if (collision.CompareTag("Player"))
            {
                OnCollisionEnter?.Invoke(attributes);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Attributes attributes))
        {
            if (collision.CompareTag("Player"))
            {
                OnCollisionExit?.Invoke(attributes);
            }
        }
    }

}
