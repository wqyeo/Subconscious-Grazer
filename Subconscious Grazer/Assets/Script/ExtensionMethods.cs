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

        float cos0 = fromTrans.DotProduct(toTrans) / (fromTrans.magnitude * toTrans.magnitude);

        return Mathf.Acos(cos0);
    }

    public static float DotProduct(this Vector2 firstV, Vector2 secondV) {
        float firstDot = firstV.x * secondV.x;
        float secondDot = firstV.y * secondV.y;

        return firstDot + secondDot;
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
