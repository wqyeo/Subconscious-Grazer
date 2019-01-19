using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalProperties {

    public static readonly Color HighlightedButtonColor;

    static GlobalProperties() {
        HighlightedButtonColor = new Color(1f, 1f, 1f, 1f);
    }
}
