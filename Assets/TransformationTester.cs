using UnityEngine;

public class TransformationTester : MonoBehaviour
{
    public Transform parent;
    public Mesh mesh;
    public Transform meshTransform;

    public Vector3 localUp;

    [ShowAsButtonInInspector]
    public void Init()
    {
        localUp = parent.InverseTransformDirection(Vector3.up);
    }

    private void OnDrawGizmos()
    {
        if (meshTransform)
        {
            Gizmos.color = new(1.0f, 0.92f, 0.016f, 0.5f);
            Gizmos.DrawMesh(mesh, meshTransform.position, meshTransform.rotation);
        }
        else
        {
            Gizmos.color = new(1.0f, 0.92f, 0.016f, 0.5f);
            Gizmos.DrawMesh(mesh);
        }

        if (parent)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(parent.position, localUp);

            Gizmos.color = Color.green;
            GizmosUtility.DrawLocalRay(Vector3.zero, Vector3.up, parent);

            Gizmos.color = Color.magenta;
            GizmosUtility.DrawLocalRay(Vector3.zero, localUp, parent);
        }
    }
}
