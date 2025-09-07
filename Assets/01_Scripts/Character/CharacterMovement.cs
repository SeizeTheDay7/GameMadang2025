using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// TODO :: Coyote Time, Jump buffer

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] CharacterStat stat;
    PlayerInput playerInput;
    Rigidbody2D body;
    Animator anim;
    SpriteRenderer sr;

    [Header("Move Parameters")]
    // [SerializeField] float moveSpeed = 7.5f;
    [SerializeField] float runMultiplier = 1.5f;

    [Header("Jump Parameters")]
    [SerializeField] float gravity = 2f;
    [SerializeField] float maxFallSpeed = 10f;
    [SerializeField] float jumpPower = 10f;

    [Header("Reverse Gravity Parameters")]
    [SerializeField] float reverseGravityCoolTime = 3f;
    [SerializeField] float reverseGravityPlatformCheckLength = 20f;
    [SerializeField] float reverseGravityInitialSpeed = 2f;

    [Header("Ground Check")]
    [SerializeField, Tooltip("플레이어 기준으로 얼만큼 떨어진 곳에 raycast 쏠 건지")] Vector3 groundCheckOffset = new Vector3(0.49f, 0, 0);
    [SerializeField, Tooltip("바닥 체크를 얼만큼 길게 할건지")] float groundCheckLength = 0.75f;
    [SerializeField] LayerMask groundLayer;
    bool grounded = false;

    [Header("actions")]
    InputAction moveAction;
    InputAction jumpAction;
    InputAction runAction;
    InputAction reverseGravityAction;

    [Header("Calculations")]
    float velX = 0;
    float velY = 0;
    float gravityDir = -1f;
    bool canReverseGravity = true;
    float colliderOffsetY;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        colliderOffsetY = GetComponent<Collider2D>().offset.y;

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        runAction = playerInput.actions["Sprint"];
        reverseGravityAction = playerInput.actions["ReverseGravity"]; // 위 아래 방향키를 Vector2로 받음
    }

    void Update()
    {
        HorizontalMove();
        VerticalMove();
    }

    void FixedUpdate()
    {
        body.linearVelocityX = velX;
        body.linearVelocityY = velY;
    }

    private void HorizontalMove()
    {
        bool pressRun = runAction.IsPressed();
        bool pressMove = moveAction.IsPressed();

        if (pressMove)
        {
            float moveInput = moveAction.ReadValue<Vector2>().x;
            // sr.flipX = moveInput < 0;
            transform.localScale = new Vector3(moveInput < 0 ? -1 : 1, transform.localScale.y, 1);

            // Animator에 Blend Tree 추가하여 Idle, Walk, Run
            if (pressRun)
            {
                anim.SetFloat("MoveValue", 1f);
                velX = moveInput * stat.moveSpeed * runMultiplier;
            }
            else
            {
                anim.SetFloat("MoveValue", 0.5f);
                velX = moveInput * stat.moveSpeed;
            }
        }
        else
        {
            anim.SetFloat("MoveValue", 0f);
            velX = 0;
        }
    }

    private void VerticalMove()
    {
        GroundCheck(); // 땅에 닿았는지 확인

        bool pressJump = jumpAction.triggered;
        bool pressRG = reverseGravityAction.triggered;
        bool platformExists = CheckPlatform();

        if (pressRG && platformExists && canReverseGravity)
        {
            Vector2 rgInput = reverseGravityAction.ReadValue<Vector2>();
            transform.localScale = new Vector3(transform.localScale.x, -rgInput.y, 1);
            gravityDir = rgInput.y;
            StartCoroutine(CooldownRG());

            if (grounded)
            {
                velY = gravityDir < 0 ? -reverseGravityInitialSpeed : reverseGravityInitialSpeed; // 땅에 붙어있을 때 중력 작용 바로 느껴지게
                transform.position += Vector3.up * (colliderOffsetY * 2 * rgInput.y);
            }
        }

        if (pressJump && grounded)
        {
            velY = jumpPower * gravityDir * -1;
        }
        else
        {
            velY += gravity * Time.deltaTime * gravityDir;
            float maxFallSpeed_G = maxFallSpeed * gravityDir;
            velY = gravityDir < 0 ? Mathf.Max(velY, maxFallSpeed_G) : Mathf.Min(maxFallSpeed_G, velY);
        }
    }

    private bool CheckPlatform()
    {
        Vector2 checkDir = gravityDir > 0 ? Vector2.down : Vector2.up;
        bool platformCheck = Physics2D.Raycast(transform.position, checkDir, reverseGravityPlatformCheckLength, groundLayer);
        Debug.DrawRay(transform.position, checkDir * reverseGravityPlatformCheckLength, Color.blue);
        return platformCheck;
    }

    private IEnumerator CooldownRG()
    {
        canReverseGravity = false;
        yield return new WaitForSeconds(reverseGravityCoolTime);
        canReverseGravity = true;
    }

    private void GroundCheck()
    {
        Vector2 checkDir = gravityDir > 0 ? Vector2.up : Vector2.down;
        bool leftCheck = Physics2D.Raycast(transform.position - groundCheckOffset, checkDir, groundCheckLength, groundLayer);
        bool rightCheck = Physics2D.Raycast(transform.position + groundCheckOffset, checkDir, groundCheckLength, groundLayer);
        Debug.DrawRay(transform.position - groundCheckOffset, checkDir * groundCheckLength, Color.red);
        Debug.DrawRay(transform.position + groundCheckOffset, checkDir * groundCheckLength, Color.red);
        if (leftCheck || rightCheck)
        {
            anim.SetBool("Grounded", true);
            grounded = true;
        }
        else
        {
            anim.SetBool("Grounded", false);
            grounded = false;
        }
    }
}
