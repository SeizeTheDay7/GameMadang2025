using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CharacterGravityAttack : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction attackAction; // 마우스 왼쪽 버튼 할당

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        attackAction = playerInput.actions["GravityAttack"];
    }

    void Update()
    {
        bool pressAttack = attackAction.triggered;
        if (pressAttack)
        {
            GravityInteractable gi = GetGravityInteractable();
        }
    }

    private GravityInteractable GetGravityInteractable()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("GravityInteractable"));
        if (hit.collider != null)
        {
            GravityInteractable gi = hit.collider.GetComponent<GravityInteractable>();
            return gi;
        }
        return null;
    }
}