using Core.Attributes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LabelTextAttribute))]
public class LabelTextAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = attribute as LabelTextAttribute;
        if (attr.Name.Length > 0)
        {
            label.text = attr.Name;
        }
        EditorGUI.PropertyField(position, property, label);
    }
}
