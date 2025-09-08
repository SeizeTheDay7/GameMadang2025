using UnityEngine;
using UnityEngine.AI;

public class HomingProjectile : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField, Min(0)] float speed;
    [SerializeField, Min(0)] float lifetime;
    Attributes character;
    float damage;


    public void Init(Attributes attributes, float damage)
    {
        TryGetComponent(out agent);
        agent.speed = speed;
        character = attributes;
        this.damage = damage;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        agent.SetDestination(character.Center);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out Attributes target))
        {
            if (target.CompareTag("Player"))
            {
                target.Heal(-damage);
                Destroy(gameObject);
            }
        }
    }
}