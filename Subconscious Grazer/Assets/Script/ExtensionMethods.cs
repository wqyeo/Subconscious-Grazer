using UnityEditor;
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

    /// <summary>
    /// Get string representation of serialized property, includes non-string fields
    /// </summary>
    public static string AsStringValue(this SerializedProperty property) {
        switch (property.propertyType) {
            case SerializedPropertyType.String:
                return property.stringValue;
            case SerializedPropertyType.Character:
            case SerializedPropertyType.Integer:
                if (property.type == "char") return System.Convert.ToChar(property.intValue).ToString();
                return property.intValue.ToString();
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";
            case SerializedPropertyType.Boolean:
                return property.boolValue.ToString();
            case SerializedPropertyType.Enum:
                return property.enumNames[property.enumValueIndex];
            default:
                return string.Empty;
        }
    }
}
