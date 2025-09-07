using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] float height = 2f;
    [SerializeField] Transform landingPos;
    public Transform GetLandingPos() => landingPos;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Obstacle");
    }

    public bool CanJumpOn(float z, float jumpHeight)
    {
        return landingPos.position.z - z <= jumpHeight;
    }
}
