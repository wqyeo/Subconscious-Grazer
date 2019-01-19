using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalProperties {

    public static readonly Color HighlightedButtonColor;

    static GlobalProperties() {
        HighlightedButtonColor = new Color(1f, 1f, 1f, 1f);
    }

    public static float GetNormalizedAngle(float angle) {
        while (angle < 0f) {
            angle += 360f;
        }

        while (360f < angle) {
            angle -= 360f;
        }

        return angle;
    }

    /// <summary>
    /// Get an angle from two position.
    /// </summary>
    public static float GetAngleFromTwoPositions(Vector2 fromTrans, Vector2 toTrans) {

        float xDistance = toTrans.x - fromTrans.x;

        float angle = (Mathf.Atan2(0, xDistance) * Mathf.Rad2Deg) - 90f;
        angle = GetNormalizedAngle(angle);

        return angle;
    }
}
