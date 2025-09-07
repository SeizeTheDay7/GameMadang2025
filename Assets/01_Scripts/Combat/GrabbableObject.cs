using System;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    [Header(" - GrabbableObject - ")]

    [Header("Components")]
    Rigidbody2D body;
    TargetJoint2D joint;
    CharacterStat characterStat;

    [Header("Calculation")]
    float initGrav;
    float attackMinSpeed;
    float attackDamage;
    Attributes owner;

    void Awake()
    {
        owner = GetComponent<Attributes>();
        joint = GetComponent<TargetJoint2D>();
        body = GetComponent<Rigidbody2D>();
        initGrav = body.gravityScale;
    }

    public void Grab(CharacterStat stat)
    {
        attackMinSpeed = stat.gravAttackMinSpeed;
        attackDamage = stat.gravAttackDamage;
        body.gravityScale = 0;
        joint.enabled = true;
    }

    public void Release()
    {
        body.gravityScale = initGrav;
        joint.enabled = false;
    }

    public void SetTarget(Vector2 target)
    {
        joint.target = target;
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