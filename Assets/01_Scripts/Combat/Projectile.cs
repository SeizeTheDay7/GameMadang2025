using System.Collections;
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
        StartCoroutine(CoDie());
    }

    private void FixedUpdate()
    {
        transform.position += speed * Time.fixedDeltaTime * transform.right;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out CharacterStat stat))
        {
            stat.TakeDamage((int)owner.Stat.Damage);
        }
    }

    private IEnumerator CoDie()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}