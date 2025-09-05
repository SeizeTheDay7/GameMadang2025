using EditorAttributes;
using UnityEngine;

public class RangeEnemy : EnemyBase
{
    [Button]
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 offset = new Vector3(0, 0.1f, 0);
        int dir = spriteRenderer.flipX ? -1 : 1;
        if(!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
        Gizmos.DrawLine(transform.position + offset, transform.position + (Vector3)(Vector2.right) * dir + offset);
    }
}
