using System;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    [Header(" - GrabbableObject - ")]

    [Header("Components")]
    public Rigidbody body { get; private set; }

    [Header("Parameters")]
    [SerializeField] float followSpeed = 10f;

    [Header("Calculation")]
    float attackMinSpeed;
    float attackDamage;
    Attributes owner;

    void Awake()
    {
        owner = GetComponent<Attributes>();
        body = GetComponent<Rigidbody>();
    }

    public void Grab(CharacterStat stat)
    {
        attackMinSpeed = stat.gravAttackMinSpeed;
        attackDamage = stat.gravAttackDamage;
        body.useGravity = false;
    }

    public void FollowTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;
        float distance = Vector3.Distance(transform.position, targetPos);
        body.linearVelocity = direction * distance * followSpeed;
    }

    public void Release()
    {
        body.useGravity = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Attributes target))
        {
            if (!owner)
            {
                Debug.LogError("No Attributes component found in the GrabbableObject.");
                target.TakeDamage(10);
                return;
            }

            bool isEnemy = target.transform.TryGetComponent(out EnemyBase enemy);
            bool fastEnough = body.linearVelocity.magnitude >= attackMinSpeed;
            // Debug.Log("(Collision occured) Speed of thrown object : " + body.linearVelocity.magnitude);

            if (isEnemy && fastEnough)
            {
                target.TakeDamage(attackDamage);
            }
        }
    }
}