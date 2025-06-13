using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(BlackboardEntryData))]
public class BlackboardEntrydataEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        SerializedProperty keyNameProp = property.FindPropertyRelative("keyName");
        SerializedProperty valueTypeProp = property.FindPropertyRelative("valueType");

        container.Add(new PropertyField(keyNameProp));
        var valueTypeField = new PropertyField(property.FindPropertyRelative("valueType"));
        container.Add(valueTypeField);

        var valueField = new VisualElement();
        container.Add(valueField);

        valueTypeField.RegisterValueChangeCallback(evt =>
        {
            valueField.Clear();

            string fieldName = "intValue";
            SerializedProperty valueTypeProp = property.FindPropertyRelative("valueType");

            switch ((BlackboardEntryData.ValueType)valueTypeProp.enumValueIndex)
            {
                case BlackboardEntryData.ValueType.Int:
                    fieldName = "intValue";
                    break;
                case BlackboardEntryData.ValueType.Float:
                    fieldName = "floatValue";
                    break;
                case BlackboardEntryData.ValueType.String:
                    fieldName = "stringValue";
                    break;
                case BlackboardEntryData.ValueType.Bool:
                    fieldName = "boolValue";
                    break;
                case BlackboardEntryData.ValueType.Vector3:
                    fieldName = "vector3Value";
                    break;
            }
            var value = property.FindPropertyRelative(fieldName);
            var field = new PropertyField(value);

            valueField.Add(field);

            field.BindProperty(value);
        });


        return container;
    }
}
