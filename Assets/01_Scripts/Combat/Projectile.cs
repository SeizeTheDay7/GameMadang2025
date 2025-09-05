using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header(" - Projectile - ")]
    [SerializeField, Min(0)] float speed;
    [SerializeField, Min(0)] float lifetime;

    Attributes owner;

    public void Init(Attributes attributes)
    {
        owner = attributes;
    }

    private void FixedUpdate()
    {
        transform.position += speed * Time.fixedDeltaTime * transform.right;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Attributes target))
        {
            if (!owner)
            {
                target.TakeDamage(10);
                return;
            }

            if (target != owner)
            {
                target.TakeDamage(owner.Stat.Damage);
                Destroy(gameObject);
            }
        }
    }
}