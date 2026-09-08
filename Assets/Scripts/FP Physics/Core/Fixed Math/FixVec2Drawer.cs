#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixVec2))]
public class FixVec2Drawer : PropertyDrawer
{
    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        SerializedProperty xProperty =
            property.FindPropertyRelative("x");

        SerializedProperty yProperty =
            property.FindPropertyRelative("y");

        SerializedProperty xRaw =
            xProperty.FindPropertyRelative("rawValue");

        SerializedProperty yRaw =
            yProperty.FindPropertyRelative("rawValue");

        float x = (float)xRaw.longValue / Fix64.ONE;
        float y = (float)yRaw.longValue / Fix64.ONE;

        Vector2 value = new Vector2(x, y);

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.BeginChangeCheck();

        value = EditorGUI.Vector2Field(
            position,
            label,
            value
        );

        if (EditorGUI.EndChangeCheck())
        {
            xRaw.longValue = (long)(value.x * Fix64.ONE);
            yRaw.longValue = (long)(value.y * Fix64.ONE);
        }

        EditorGUI.EndProperty();
    }
}

#endif