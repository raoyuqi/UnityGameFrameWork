using FrameWork.Core.Attributes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LabelEnumAttribute))]
public class LabelEnumAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = attribute as LabelEnumAttribute;
        if (attr.Name.Length > 0)
            label.text = attr.Name;

        //var isElement = Regex.IsMatch(property.displayName, "Element \\d+");
        //if (isElement)
        //    label.text = property.displayName;

        if (property.propertyType == SerializedPropertyType.Enum)
            this.DrawEnum(position, property, label);
        else
            EditorGUI.PropertyField(position, property, label);
    }

    public void DrawEnum(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();

        var type = fieldInfo.FieldType;
        var names = property.enumNames;
        var values = new string[names.Length];

        while (type.IsArray)
            type = type.GetElementType();

        // 获取枚举所对应的名称
        for (int i = 0; i < names.Length; i++)
        {
            var info = type.GetField(names[i]);
            var attrs = (LabelEnumAttribute[])info.GetCustomAttributes(typeof(LabelEnumAttribute), false);
            values[i] = attrs.Length == 0 ? names[i] : attrs[0].Name;
        }

        // 重绘GUI
        var index = EditorGUI.Popup(position, label.text, property.enumValueIndex, values);
        if (EditorGUI.EndChangeCheck() && index != -1)
            property.enumValueIndex = index;

    }
}

