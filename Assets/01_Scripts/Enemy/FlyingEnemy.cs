using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemy : MonoBehaviour
{
    [Header(" - Attributes - ")]
    protected Attributes attributes;
    [SerializeField] protected float attackRange;

    [Header(" - State - ")]
    [SerializeField] protected EnemyState currentState = EnemyState.Idle;
    bool nearPlayer = false;

    [Header(" - Locomotion - ")]
    NavMeshAgent agent;
    [SerializeField] protected float patrolSpeed;
    /*    [SerializeField] protected PatrolPoint[] patrolPoints;
        [SerializeField] protected ChaseRangeCollider chaseRangeCollider;
        [SerializeField] protected float chaseRange;
        int currentPatrolIndex = 0;*/

    [Header(" - Rendering - ")]
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [Header(" - Animation - ")]
    [SerializeField] protected Animator animator;

    [Header(" - Character - ")]
    [SerializeField] protected Attributes character;
    public void SetCharacter(Attributes character) => this.character = character;

    protected virtual void Awake()
    {
        TryGetComponent(out attributes);
        TryGetComponent(out agent);

        /*        if (!chaseRangeCollider)
                    chaseRangeCollider = GetComponentInChildren<ChaseRangeCollider>();*/

        if (!spriteRenderer)
            TryGetComponent(out spriteRenderer);

        //  chaseRangeCollider.SetRadius(chaseRange);
    }

    /*    private void OnEnable()
        {
            chaseRangeCollider.OnCollisionEnter += AttackRangeCollider_OnCollisionEnter;
            chaseRangeCollider.OnCollisionExit += AttackRangeCollider_OnCollisionExit;
        }
        private void OnDisable()
        {
            chaseRangeCollider.OnCollisionEnter -= AttackRangeCollider_OnCollisionEnter;
            chaseRangeCollider.OnCollisionExit -= AttackRangeCollider_OnCollisionExit;
        }*/

    /*    private void AttackRangeCollider_OnCollisionEnter(Attributes character)
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
        }*/

    private void Start()
    {
        if (attributes.Stat.AttackCooldownTime == null || attributes.Stat.IdleTimeWait == null)
            attributes.Stat.Init();

        ChangeState(EnemyState.Chase);
    }

    protected virtual void Update()
    {
        if (agent.velocity.x > 0.1f)
            spriteRenderer.flipX = false;
        else if (agent.velocity.x < -0.1f)
            spriteRenderer.flipX = true;

        if (currentState == EnemyState.Patrol)
        {
            if (agent.remainingDistance < 0.1f)
            {
                StartCoroutine(CoIdle());
            }
        }
        else if (currentState == EnemyState.Chase)
        {
            agent.SetDestination(character.transform.position);

            float distanceToCharacter = Vector3.Distance(transform.position, character.transform.position);

            /*            if (distanceToCharacter > chaseRange * 1.15f)
                        {
                            StopAllCoroutines();
                            ChangeState(EnemyState.Patrol);
                        }*/
            if (distanceToCharacter < attackRange)
            {
                if (!Physics.Raycast(transform.position, character.transform.position - transform.position, out RaycastHit hit, attackRange, 1 << 6))
                {
                    ChangeState(EnemyState.Attack);
                }
            }
            CheckCharacterNotice();
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
                agent.SetDestination(transform.position);
                break;
            case EnemyState.Patrol:
                agent.speed = patrolSpeed;
                /*                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                                PatrolPoint patrolPoint = patrolPoints[currentPatrolIndex];
                                agent.SetDestination(patrolPoint.transform.position);*/
                break;
            case EnemyState.Chase:
                agent.speed = patrolSpeed * 1.5f;
                break;
            case EnemyState.Attack:
                agent.SetDestination(transform.position);
                Attack();
                break;
            case EnemyState.Dead:
                agent.SetDestination(transform.position);
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

    bool IsLookingAtPlayer()
    {
        if (!character) return false;
        int dir = spriteRenderer.flipX ? -1 : 1;
        Vector3 toPlayer = (character.Center - transform.position).normalized;
        return Vector3.Dot(toPlayer, Vector3.right * dir) > 0.5f;
    }

}
