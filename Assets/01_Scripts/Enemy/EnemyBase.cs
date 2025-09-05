using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header(" - Attributes - ")]
    protected Attributes attributes;
    protected bool canAttack = true;
    [SerializeField] protected AttackRangeCollider attackRangeCollider;
    [SerializeField] protected float attackRange;

    [Header(" - Debug - ")]
    [SerializeField] protected Attributes character;

    protected virtual void Awake()
    {
        TryGetComponent(out attributes);

        if (!attackRangeCollider)
            attackRangeCollider = GetComponentInChildren<AttackRangeCollider>();

        attackRangeCollider.SetRadius(attackRange);
    }

    private void OnEnable()
    {
        attackRangeCollider.OnCollision += AttackRangeCollider_OnCollision;
    }
    private void OnDisable()
    {
        attackRangeCollider.OnCollision -= AttackRangeCollider_OnCollision;
    }

    private void Start()
    {
        if (attributes.Stat.AttackCooldownTime == null)
            attributes.Stat.Init();
       
    }

    private void AttackRangeCollider_OnCollision(Attributes character)
    {
        this.character = character;
    }

    protected virtual void Update()
    {
        if (character && canAttack)
            Attack();
    }

    protected abstract void Attack();

    protected IEnumerator CoAttack()
    {
        canAttack = false;
        Vector2 direction = (character.Center - transform.position).normalized;
        var projectile = Instantiate(attributes.Stat.Projectile, transform.position, Quaternion.identity);
        projectile.transform.right = direction;
        projectile.Init(attributes);
        yield return attributes.Stat.AttackCooldownTime;
        canAttack = true;
    }

}
