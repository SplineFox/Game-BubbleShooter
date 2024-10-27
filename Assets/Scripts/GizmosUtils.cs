using UnityEngine;

public static class GizmosUtils
{
    private static float _arrowHeadLength = 0.25f;
    private static float _arrowHeadAngle = 20;

    public static void DrawArrow(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 direction = endPoint - startPoint;
        if (direction.magnitude == 0)
            return;

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(180 + _arrowHeadAngle, 0, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(180 - _arrowHeadAngle, 0, 0) * Vector3.forward;

        Gizmos.DrawLine(startPoint, endPoint);
        Gizmos.DrawRay(endPoint, right * _arrowHeadLength);
        Gizmos.DrawRay(endPoint, left * _arrowHeadLength);
    }
}
