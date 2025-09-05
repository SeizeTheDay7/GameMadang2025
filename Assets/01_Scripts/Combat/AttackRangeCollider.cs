using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AttackRangeCollider : MonoBehaviour
{
    public Action<Attributes> OnCollision;
    public void SetRadius(float radius)
    {
        if (TryGetComponent<CircleCollider2D>(out var collider))
        {
            collider.radius = radius;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Attributes>(out var attributes))
        {
            if (collision.CompareTag("Player"))
            {
                OnCollision?.Invoke(attributes);
            }
        }
    }

}
