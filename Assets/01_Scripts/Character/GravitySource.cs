using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class GravitySource : MonoBehaviour
{
    // 클릭하고 있을 때 활성화되며 주변의 물체를 빨아들인다
    CircleCollider2D col;
    public Vector2 additionalAccel { get; private set; }
    [SerializeField] float additionalAccelMultiplier = 4f;
    public float endFollowingDist = 5f;
    public bool isActivated { get; private set; } = false;
    Vector3 preMousePos;
    public event System.Action OnTurnedOff;

    void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        col.enabled = false;
    }

    public void TurnOn()
    {
        isActivated = true;
        col.enabled = true;
        preMousePos = GetCurrentMouseWorldPos();
        additionalAccel = Vector2.zero;
    }

    public void TurnOff()
    {
        isActivated = false;
        col.enabled = false;
        OnTurnedOff?.Invoke();
    }

    void Update()
    {
        if (!isActivated) return;
        transform.position = GetCurrentMouseWorldPos();
        additionalAccel = (GetCurrentMouseWorldPos() - preMousePos) / Time.deltaTime * additionalAccelMultiplier;
        Debug.Log("addtionalAccel : " + additionalAccel);
        preMousePos = GetCurrentMouseWorldPos();
    }

    private Vector3 GetCurrentMouseWorldPos()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }
}