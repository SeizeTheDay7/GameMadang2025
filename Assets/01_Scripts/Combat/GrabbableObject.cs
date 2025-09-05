using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    [Header(" - GrabbableObject - ")]
    [SerializeField] float attackableSpeed = 10f;
    Attributes owner;
    Rigidbody2D body;

    void Awake()
    {
        owner = GetComponent<Attributes>();
        body = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Attributes target))
        {
            if (!owner)
            {
                target.TakeDamage(10);
                return;
            }

            bool isEnemy = target.transform.TryGetComponent(out EnemyBase enemy);
            bool fastEnough = body.linearVelocity.magnitude >= attackableSpeed;
            Debug.Log("(Collision occured) Speed of thrown object : " + body.linearVelocity.magnitude);

            if (isEnemy && fastEnough)
            {
                target.TakeDamage(owner.Stat.Damage);
            }
        }
    }
}