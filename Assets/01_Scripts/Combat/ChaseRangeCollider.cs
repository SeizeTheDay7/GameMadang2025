using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChaseRangeCollider : MonoBehaviour
{
    public Action<Attributes> OnCollisionEnter;
    public Action<Attributes> OnCollisionExit;
    public void SetRadius(float radius)
    {
        if (TryGetComponent<SphereCollider>(out var collider))
        {
            collider.radius = radius;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out Attributes attributes))
        {
            if (collision.CompareTag("Player"))
            {
                OnCollisionEnter?.Invoke(attributes);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
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
