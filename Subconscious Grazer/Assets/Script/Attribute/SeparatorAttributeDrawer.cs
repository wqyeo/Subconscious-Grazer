﻿using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(SeparatorAttribute))]
public class SeparatorAttributeDrawer : DecoratorDrawer {

    private SeparatorAttribute SeparatorAttributeProp { get { return ((SeparatorAttribute)attribute); } }

    public override void OnGUI(Rect _position) {
        if (SeparatorAttributeProp.Title == "") {
            _position.height = 1;
            _position.y += 19;
            GUI.Box(_position, "");
        } else {
            Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(SeparatorAttributeProp.Title));
            float separatorWidth = (_position.width - textSize.x) / 2.0f - 5.0f;
            _position.y += 19;

            GUI.Box(new Rect(_position.xMin, _position.yMin, separatorWidth, 1), "");
            GUI.Label(new Rect(_position.xMin + separatorWidth + 5.0f, _position.yMin - 8.0f, textSize.x, 20), SeparatorAttributeProp.Title);
            GUI.Box(new Rect(_position.xMin + separatorWidth + 10.0f + textSize.x, _position.yMin, separatorWidth, 1), "");
        }
    }

    public override float GetHeight() {
        return SeparatorAttributeProp.WithOffset ? 36.0f : 26f;
    }
}