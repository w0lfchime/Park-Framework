#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Fix64))]
public class Fix64Drawer : PropertyDrawer
{
    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        SerializedProperty raw =
            property.FindPropertyRelative("rawValue");

        float value = (float)raw.longValue / Fix64.ONE;

        EditorGUI.BeginChangeCheck();

        value = EditorGUI.FloatField(
            position,
            label,
            value
        );

        if (EditorGUI.EndChangeCheck())
        {
            raw.longValue = (long)(value * Fix64.ONE);
        }
    }
}

#endif