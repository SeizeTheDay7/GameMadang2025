using System.Collections;
using UnityEngine;

public enum EnemyState
{
    None = -1,

    Idle = 0,
    Patrol = 1,
    Chase = 2,
    Jump = 3,
    Attack = 4,
    Dead = 5,

    Max
}

public abstract class EnemyBase : MonoBehaviour
{
    [Header(" - Attributes - ")]
    protected Attributes attributes;
    [SerializeField] protected float attackRange;

    [Header(" - State - ")]
    [SerializeField] protected EnemyState currentState = EnemyState.Idle;
    EnemyState lastState;
    bool nearPlayer = false;

    [Header(" - Locomotion - ")]
    [SerializeField] protected float patrolSpeed;
    protected Vector3 moveDirection;
    [SerializeField] protected PatrolPoint[] patrolPoints;
    [SerializeField] protected ChaseRangeCollider chaseRangeCollider;
    [SerializeField] protected float chaseRange;
    int currentPatrolIndex = 0;

    [SerializeField] float jumpHeight;
    Obstacle obstacle;
    Vector3 landingPos;

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
        transform.position += speed * Time.deltaTime * moveDirection;
        spriteRenderer.flipX = moveDirection.x < 0;

        float distanceToPoint = Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].transform.position);
        if (distanceToPoint < 0.1f && currentState == EnemyState.Patrol)
        {
            StartCoroutine(CoIdle());
            return;
        }

        CheckGround();
        CheckForObstacle();

        if (currentState != EnemyState.Chase) return;

        float distanceToCharacter = Vector3.Distance(transform.position, character.Center);
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
                moveDirection = Vector3.zero;
                break;
            case EnemyState.Patrol:
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                PatrolPoint patrolPoint = patrolPoints[currentPatrolIndex];
                moveDirection = (patrolPoint.transform.position - transform.position).normalized;
                moveDirection.z = 0;
                break;
            case EnemyState.Chase:
                StartCoroutine(CoChase());
                break;
            case EnemyState.Jump:
                StartCoroutine(CoJump());
                break;
            case EnemyState.Attack:
                moveDirection = Vector3.zero;
                Attack();
                break;
            case EnemyState.Dead:
                moveDirection = Vector3.zero;
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
            moveDirection.z = 0;
            yield return null;
        }
    }

    protected virtual IEnumerator CoJump()
    {
        Vector3 startPos = transform.position;
        landingPos = obstacle ? obstacle.GetLandingPos().position : landingPos;
        float duration = 0.5f; // ���� �ð� (���� ����)
        float elapsed = 0f;
        moveDirection = Vector3.zero;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 parabola = Vector3.Lerp(startPos, landingPos, t) + 1.1f * Mathf.Sin(Mathf.PI * t) * Vector3.forward;
            transform.position = parabola;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = landingPos;

        if (lastState == EnemyState.Patrol)
            currentPatrolIndex--;

        obstacle = null;

        ChangeState(lastState);
    }

    void CheckGround()
    {
        if (currentState == EnemyState.Jump) return;

        if (currentState == EnemyState.Chase && character.transform.position.z > transform.position.z) return;

        if (currentState == EnemyState.Patrol && patrolPoints[currentPatrolIndex].transform.position.z > transform.position.z) return;


        if (Physics.Raycast(transform.position + Vector3.forward, Vector3.back, out RaycastHit hit, 10f, 1 << 8 | 1 << 6))
        {
            if (hit.point.z >= transform.position.z - 0.1f)
            {
                return;
            }

            landingPos = hit.point + moveDirection * .5f;
            print(landingPos);

            ChangeState(EnemyState.Jump);
        }
    }

    void CheckForObstacle()
    {
        if (Physics.Raycast(transform.position + Vector3.forward * .5f, moveDirection, out RaycastHit hit, 0.5f, 1 << 8))
        {
            if (hit.collider.TryGetComponent(out obstacle))
            {
                if (obstacle.CanJumpOn(transform.position.z, jumpHeight))
                {
                    lastState = currentState;
                    ChangeState(EnemyState.Jump);
                }
            }
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
