using System.Collections;
using UnityEngine;

public enum EnemyState
{
    None = -1,

    Idle = 0,
    Patrol = 1,
    Chase = 2,
    Attack = 3,
    Dead = 4,

    Max
}

public abstract class EnemyBase : MonoBehaviour
{
    [Header(" - Attributes - ")]
    protected Attributes attributes;
    [SerializeField] protected float attackRange;

    [Header(" - State - ")]
    [SerializeField] protected EnemyState currentState = EnemyState.Idle;
    bool nearPlayer = false;

    [Header(" - Locomotion - ")]
    [SerializeField] protected float patrolSpeed;
    protected Vector2 moveDirection;
    [SerializeField] protected PatrolPoint[] patrolPoints;
    [SerializeField] protected ChaseRangeCollider chaseRangeCollider;
    [SerializeField] protected float chaseRange;
    int currentPatrolIndex = 0;

    [Header(" - Rendering - ")]
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [Header(" - Animation - ")]
    [SerializeField] protected Animator animator;

    [Header(" - Debug - ")]
    [SerializeField] protected Attributes character;

    protected virtual void Awake()
    {
        TryGetComponent(out attributes);

        if (!chaseRangeCollider)
            chaseRangeCollider = GetComponentInChildren<ChaseRangeCollider>();

        if (!spriteRenderer)
            TryGetComponent(out spriteRenderer);

        chaseRangeCollider.SetRadius(chaseRange);
    }

    private void OnEnable()
    {
        chaseRangeCollider.OnCollisionEnter += AttackRangeCollider_OnCollisionEnter;
        chaseRangeCollider.OnCollisionExit += AttackRangeCollider_OnCollisionExit;
    }


    private void OnDisable()
    {
        chaseRangeCollider.OnCollisionEnter -= AttackRangeCollider_OnCollisionEnter;
        chaseRangeCollider.OnCollisionExit -= AttackRangeCollider_OnCollisionExit;
    }
    private void AttackRangeCollider_OnCollisionEnter(Attributes character)
    {
        this.character = character;
        nearPlayer = true;

        if (!IsLookingAtPlayer()) return;

        ChangeState(EnemyState.Chase);
    }

    private void AttackRangeCollider_OnCollisionExit(Attributes attributes)
    {
        nearPlayer = false;
        ChangeState(EnemyState.Patrol);
    }

    private void Start()
    {
        if (attributes.Stat.AttackCooldownTime == null || attributes.Stat.IdleTimeWait == null)
            attributes.Stat.Init();

        ChangeState(EnemyState.Patrol);
    }

    protected virtual void Update()
    {
        Move();
        CheckCharacterNotice();
    }

    private void Move()
    {
        float speed = currentState == EnemyState.Chase ? patrolSpeed * 1.5f : patrolSpeed;
        transform.position += (Vector3)(speed * Time.deltaTime * moveDirection);
        spriteRenderer.flipX = moveDirection.x < 0;

        float distanceToPoint = Vector2.Distance(transform.position, patrolPoints[currentPatrolIndex].transform.position);
        if (distanceToPoint < 0.1f && currentState == EnemyState.Patrol)
        {
            StartCoroutine(CoIdle());
            return;
        }

        if (currentState != EnemyState.Chase) return;

        float distanceToCharacter = Vector2.Distance(transform.position, character.Center);
        if (distanceToCharacter > chaseRange * 1.15f)
        {
            StopAllCoroutines();
            ChangeState(EnemyState.Patrol);
        }
        else if (distanceToCharacter < attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
    }

    private void CheckCharacterNotice()
    {
        if (currentState != EnemyState.Idle && currentState != EnemyState.Patrol) return;
        if (nearPlayer && IsLookingAtPlayer()) ChangeState(EnemyState.Chase);
    }

    protected virtual void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Idle:
                moveDirection = Vector2.zero;
                break;
            case EnemyState.Patrol:
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                PatrolPoint patrolPoint = patrolPoints[currentPatrolIndex];
                moveDirection = (patrolPoint.transform.position - transform.position).normalized;
                moveDirection.y = 0;
                break;
            case EnemyState.Chase:
                StartCoroutine(CoChase());
                break;
            case EnemyState.Attack:
                moveDirection = Vector2.zero;
                Attack();
                break;
            case EnemyState.Dead:
                moveDirection = Vector2.zero;
                break;
            default:
                break;
        }

    }

    protected virtual void Attack()
    {
        StopAllCoroutines();

        if (Vector2.Distance(transform.position, character.Center) > attackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        StartCoroutine(CoAttack());
    }

    protected IEnumerator CoAttack()
    {
        Vector2 direction = (character.Center - transform.position).normalized;
        var projectile = Instantiate(attributes.Stat.Projectile, transform.position, Quaternion.identity);
        projectile.transform.right = direction;
        projectile.Init(attributes);
        yield return attributes.Stat.AttackCooldownTime;
        Attack();
    }

    protected virtual IEnumerator CoIdle()
    {
        ChangeState(EnemyState.Idle);
        yield return attributes.Stat.IdleTimeWait;
        ChangeState(EnemyState.Patrol);
    }

    protected virtual IEnumerator CoChase()
    {
        while (currentState == EnemyState.Chase)
        {
            moveDirection = (character.Center - transform.position).normalized;
            moveDirection.y = 0;
            yield return null;
        }
    }

    bool IsLookingAtPlayer()
    {
        if (!character) return false;
        int dir = spriteRenderer.flipX ? -1 : 1;
        Vector3 toPlayer = (character.Center - transform.position).normalized;
        return Vector2.Dot(toPlayer, Vector2.right * dir) > 0.5f;
    }

}
