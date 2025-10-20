// Editor/BigNumDrawer.cs
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BigNum))]
public class BigNumDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property == null) return;

        SerializedProperty mantissaProp = property.FindPropertyRelative("m");
        SerializedProperty exponentProp = property.FindPropertyRelative("e");

        EditorGUI.BeginProperty(position, label, property);

        if (mantissaProp == null || exponentProp == null)
        {
            EditorGUI.LabelField(position, label.text, "BigNum: fields not found");
            EditorGUI.EndProperty();
            return;
        }

        float labelWidth = EditorGUIUtility.labelWidth;
        float spacing = 4f;
        float fieldWidth = (position.width - labelWidth - spacing) / 2f;

        Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
        Rect mantissaRect = new Rect(position.x + labelWidth, position.y, fieldWidth, position.height);
        Rect exponentRect = new Rect(mantissaRect.x + fieldWidth + spacing, position.y, fieldWidth - spacing, position.height);

        EditorGUI.LabelField(labelRect, label);

        EditorGUI.PropertyField(mantissaRect, mantissaProp, GUIContent.none);
        EditorGUI.PropertyField(exponentRect, exponentProp, GUIContent.none);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
