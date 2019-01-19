using UnityEngine;

public static class ExtensionMethods {

    public static float GetNormalizedAngle(this float angle) {
        while (angle < 0f) {
            angle += 360f;
        }

        while (360f < angle) {
            angle -= 360f;
        }

        return angle;
    }

    public static float GetAngleToPosition(this Vector2 fromTrans, Vector2 toTrans) {

        float xDistance = toTrans.x - fromTrans.x;

        float angle = (Mathf.Atan2(0, xDistance) * Mathf.Rad2Deg) - 90f;
        angle = GetNormalizedAngle(angle);

        return angle;
    }

    /// <summary>
    /// Rotates this vector around the given degree.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="degrees"></param>
    /// <returns></returns>
    public static Vector2 Rotate(this Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;

        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);

        return v;
    }
}
