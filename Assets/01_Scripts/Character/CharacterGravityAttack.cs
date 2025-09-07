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
        }

        bool pressAttack = attackAction.IsPressed();
        if (pressAttack && grabbedObj)
        {
            grabbedObj.SetTarget(GetCurrentMousePos());
        }
    }

    private GrabbableObject GetGrabbableObject()
    {
        Collider2D hit = Physics2D.OverlapPoint(GetCurrentMousePos(), interactableLayer);
        return hit?.GetComponent<GrabbableObject>();
    }

    private Vector2 GetCurrentMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}