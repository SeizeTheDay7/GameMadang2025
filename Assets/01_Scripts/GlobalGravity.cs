using UnityEngine;

public class GlobalGravity : MonoBehaviour
{
    [SerializeField] float gravityScale = 2f;

    void Awake()
    {
        Physics.gravity = new Vector3(0, 0, -9.81f * gravityScale);
    }
}