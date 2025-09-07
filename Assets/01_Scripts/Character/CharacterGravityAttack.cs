using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CharacterGravityAttack : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] CharacterStat stat;
    PlayerInput playerInput;
    InputAction attackAction; // 마우스 왼쪽 버튼 할당

    [Header("Parameters")]
    [SerializeField] LayerMask interactableLayer;

    [Header("Calculation")]
    GrabbableObject grabbedObj = null;
    Vector3 targetPos;

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
            grabbedObj = GetGrabbableObject();
            if (grabbedObj == null) return;
            grabbedObj.Grab(stat);
        }
        else if (releaseAttack && grabbedObj != null)
        {
            grabbedObj.Release();
            grabbedObj = null;
        }

        bool pressAttack = attackAction.IsPressed();
        if (pressAttack)
        {
            targetPos = GetCurrentMousePos();
            targetPos.y = 0;
        }
    }

    void FixedUpdate()
    {
        grabbedObj?.FollowTarget(targetPos);
    }

    private GrabbableObject GetGrabbableObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(ray.origin, ray.direction * 30f, Color.red, 1f);

        RaycastHit hit; if (Physics.Raycast(ray, out hit, 30f, interactableLayer))
        {
            return hit.collider.GetComponent<GrabbableObject>();
        }
        return null;
    }

    private Vector3 GetCurrentMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}