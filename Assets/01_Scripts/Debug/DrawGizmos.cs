using UnityEngine;
using EditorAttributes;

public class DrawGizmos : MonoBehaviour
{
    [SerializeField] private GizmoType gizmoType = GizmoType.WireSphere;
    [SerializeField] private float size = 1f;
    [SerializeField] private Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        switch (gizmoType)
        {
            case GizmoType.WireSphere:
                Gizmos.color = gizmoColor;
                Gizmos.DrawWireSphere(transform.position, size);
                break;
            case GizmoType.Sphere:
                Gizmos.color = gizmoColor;
                Gizmos.DrawSphere(transform.position, size);
                break;
            case GizmoType.Cube:
                Gizmos.color = gizmoColor;
                Gizmos.DrawCube(transform.position, Vector3.one * size);
                break;
            case GizmoType.WireCube:
                Gizmos.color = gizmoColor;
                Gizmos.DrawWireCube(transform.position, Vector3.one * size);
                break;
        }
    }
}

public enum GizmoType
{
    WireSphere,
    Sphere,
    Cube,
    WireCube
}