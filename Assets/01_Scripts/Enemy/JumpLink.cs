using UnityEngine;

public class JumpLink : MonoBehaviour
{
    public JumpLink upJumpLink;
    public JumpLink downJumpLink;
    Vector3 offset = new Vector3(0, 0, 0.5f);
    GameObject player;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("JumpLink");
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out EnemyBase enemy))
        {
            if (Mathf.Abs(other.transform.position.z - player.transform.position.z) < 0.5f)
            {
                return;
            }

            if (other.transform.position.z < player.transform.position.z)
            {
                if (!upJumpLink) return;
                enemy.Jump(upJumpLink.transform.position - offset);
            }
            else
            {
                if (!downJumpLink) return;
                enemy.Jump(downJumpLink.transform.position - offset);
            }
        }
    }
    void OnDrawGizmos()
    {
        if (upJumpLink)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position - offset, upJumpLink.transform.position - offset);
        }

        if (downJumpLink)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position - offset, downJumpLink.transform.position - offset);
        }
    }
}
