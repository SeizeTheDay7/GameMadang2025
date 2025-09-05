using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityInteractable : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float gravity = 10f; // 평소 중력
    [SerializeField] float followingAcceleration = 10f; // 빨려가는 속력

    [Header("Components")]
    Rigidbody2D body;

    [Header("Calculation")]
    GravitySource gravitySource;
    bool isFollowing = false;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        bool isGravitySource = collision.CompareTag("GravitySource");
        gravitySource = collision.GetComponent<GravitySource>();
        bool sourceActivated = gravitySource.isActivated;

        if (isGravitySource && sourceActivated)
        {
            gravitySource.OnTurnedOff += EndFollowing;
            StartFollowing();
        }
    }

    public void StartFollowing()
    {
        isFollowing = true;
    }

    // 종료 조건 : 마우스 클릭 끝남, 일정 거리 이상 멀어짐
    public void EndFollowing()
    {
        isFollowing = false;
        gravitySource.OnTurnedOff -= EndFollowing;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        bool isGravitySource = collision.CompareTag("GravitySource");

        if (isGravitySource)
        {
            isFollowing = false;
        }
    }

    void FixedUpdate()
    {
        if (isFollowing)
        {
            Vector2 dir = (gravitySource.transform.position - transform.position).normalized;
            body.linearVelocity += dir * followingAcceleration * Time.fixedDeltaTime;
            body.linearVelocity += gravitySource.additionalAccel;

            Vector2 distFromSource = transform.position - gravitySource.transform.position;
            if (distFromSource.magnitude > gravitySource.endFollowingDist)
            {
                EndFollowing();
            }
        }
        else
        {
            body.linearVelocity += Vector2.down * gravity * Time.fixedDeltaTime;
        }
    }

}