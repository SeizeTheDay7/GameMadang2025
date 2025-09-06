using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CharacterGravityAttack : MonoBehaviour
{
    [Header("Components")]
    PlayerInput playerInput;
    InputAction attackAction; // 마우스 왼쪽 버튼 할당

    [Header("Parameters")]
    [SerializeField] float cancelDistance = 5f; // 홀드가 취소되는 거리
    [SerializeField] LayerMask interactableLayer;

    [Header("Calculation")]
    Transform grabbedObject = null;
    TargetJoint2D targetJoint;
    float prevGravityScale;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        attackAction = playerInput.actions["GravityAttack"];
    }

    void Update()
    {
        bool triggerAttack = attackAction.triggered;
        bool releaseAttack = attackAction.WasReleasedThisFrame();

        if (triggerAttack)
        {
            grabbedObject = GetGravityInteractable();

            if (grabbedObject == null) return;

            Rigidbody2D grabbedObjectRB = grabbedObject.GetComponent<Rigidbody2D>();
            prevGravityScale = grabbedObjectRB.gravityScale;
            grabbedObjectRB.gravityScale = 0;

            targetJoint = grabbedObject.GetComponent<TargetJoint2D>();
            targetJoint.enabled = true;
        }
        else if (releaseAttack && grabbedObject != null)
        {
            ReleaseAttack();
        }

        Vector2 mousePos = GetCurrentMousePos();
        bool pressAttack = attackAction.IsPressed();
        bool tooFarFromCharacter = Vector2.Distance(transform.position, mousePos) > cancelDistance;

        if (pressAttack)
        {
            if (grabbedObject == null) return;
            if (tooFarFromCharacter) { ReleaseAttack(); return; }

            targetJoint.target = mousePos;
        }
    }

    private void ReleaseAttack()
    {
        grabbedObject.GetComponent<Rigidbody2D>().gravityScale = prevGravityScale;
        targetJoint.enabled = false;
        targetJoint = null;
        grabbedObject = null;
    }

    private Transform GetGravityInteractable()
    {
        Collider2D hit = Physics2D.OverlapPoint(GetCurrentMousePos(), interactableLayer);
        return hit?.transform;
    }

    private Vector2 GetCurrentMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}