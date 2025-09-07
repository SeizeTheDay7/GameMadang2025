using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [Header(" - Projectile - ")]
    [SerializeField, Min(0)] float speed;
    [SerializeField, Min(0)] float lifetime;

    Attributes owner;

    public void Init(Attributes attributes)
    {
        owner = attributes;
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        transform.position += speed * Time.fixedDeltaTime * transform.right;
    }

    private void OnTriggerEnter(Collider collision)
    {
        // if (collision.TryGetComponent(out Attributes target))
        // {
        //     if (!owner && collision.CompareTag("Player"))
        //     {
        //         target.TakeDamage(10);
        //         return;
        //     }

        //     if (target != owner && collision.CompareTag("Player"))
        //     {
        //         target.TakeDamage(owner.Stat.Damage);
        //         Destroy(gameObject);
        //     }
        // }
        if (collision.TryGetComponent(out CharacterStat stat))
        {
            stat.TakeDamage((int)owner.Stat.Damage);
            Destroy(gameObject);
        }
    }

    private IEnumerator CoDie()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}