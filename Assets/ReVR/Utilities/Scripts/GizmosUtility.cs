using UnityEngine;

public static class GizmosUtility
{
    public const float DefaultCylinderRadius = 0.5f;
    public const float DefaultCylinderHeight = 1.0f;
    public const int DefaultCylinderResolution = 10;

    public static void DrawCylinder(Vector3 center, Vector3 axis)
    {
        DrawCylinder(center, axis, DefaultCylinderRadius, DefaultCylinderHeight, DefaultCylinderResolution);
    }

    public static void DrawCylinder(Vector3 center, Vector3 axis, float radius)
    {
        DrawCylinder(center, axis, radius, DefaultCylinderHeight, DefaultCylinderResolution);
    }

    public static void DrawCylinder(Vector3 center, Vector3 axis, float radius, float height)
    {
        DrawCylinder(center, axis, radius, height, DefaultCylinderResolution);
    }

    public static void DrawCylinder(Vector3 center, Vector3 axis, float radius, float height, int resolution, bool useHandle = false)
    {
#if UNITY_EDITOR
        resolution = Mathf.Max(resolution, 1);

        float deltaAngle = Mathf.PI * 2.0f / resolution;
        Vector3 up = height * 0.5f * axis.normalized;
        Vector3 topCenter = center + up;
        Vector3 bottomCenter = center - up;

        Vector3 forward = Vector3.Slerp(axis, Vector3.right, 0.5f);
        if (forward == Vector3.right)
            forward = Vector3.forward;

        Vector3 right = Vector3.Cross(axis, forward).normalized * radius;
        forward = Vector3.Cross(axis, right).normalized * radius;

        if (useHandle)
        {
            UnityEditor.Handles.color = Gizmos.color;
            UnityEditor.Handles.DrawWireDisc(topCenter, axis, radius);
            UnityEditor.Handles.DrawWireDisc(bottomCenter, axis, radius);

            for (int i = 0; i < resolution; i++)
            {
                float angleA = deltaAngle * i;
                Vector3 offsetA = Mathf.Cos(angleA) * right + Mathf.Sin(angleA) * forward;

                float angleB = deltaAngle * (i + 1);
                Vector3 offsetB = Mathf.Cos(angleB) * right + Mathf.Sin(angleB) * forward;

                Vector3 topPointA = topCenter + offsetA;
                Vector3 bottomPointA = bottomCenter + offsetA;
                UnityEditor.Handles.DrawLine(topPointA, bottomPointA);
            }
        }
        else
        {
            for (int i = 0; i < resolution; i++)
            {
                float angleA = deltaAngle * i;
                Vector3 offsetA = Mathf.Cos(angleA) * right + Mathf.Sin(angleA) * forward;

                float angleB = deltaAngle * (i + 1);
                Vector3 offsetB = Mathf.Cos(angleB) * right + Mathf.Sin(angleB) * forward;

                Vector3 topPointA = topCenter + offsetA;
                Vector3 topPointB = topCenter + offsetB;
                Gizmos.DrawLine(topPointA, topPointB);

                Vector3 bottomPointA = bottomCenter + offsetA;
                Vector3 bottomPointB = bottomCenter + offsetB;
                Gizmos.DrawLine(bottomPointA, bottomPointB);

                Gizmos.DrawLine(topPointA, bottomPointA);
            }
        }
#endif
    }

    public static void DrawLocalRay(Vector3 localPosition, Vector3 localDirection, Transform transform)
    {
        Vector3 worldPosition = (transform == null) ? localPosition : transform.TransformPoint(localPosition);
        Vector3 worldDirection = (transform == null) ? localDirection : transform.TransformDirection(localDirection);
        
        Gizmos.DrawRay(worldPosition, worldDirection);
    }

    public static void DrawLocalRay(Ray localRay, Transform transform)
    {
        Vector3 worldPosition = (transform == null) ? localRay.origin : transform.TransformPoint(localRay.origin);
        Vector3 worldDirection = (transform == null) ? localRay.direction : transform.TransformDirection(localRay.direction);

        Gizmos.DrawRay(worldPosition, worldDirection);
    }

}