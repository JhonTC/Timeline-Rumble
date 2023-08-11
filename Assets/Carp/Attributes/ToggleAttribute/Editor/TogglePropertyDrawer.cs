using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Carp.Attributes
{
    [CustomPropertyDrawer(typeof(ToggleAttribute))]
    public class TogglePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ToggleAttribute toggle = attribute as ToggleAttribute;

            GUIContent originalLabel = new GUIContent(label);
            EditorGUI.BeginProperty(position, label, property);
            
            if (toggle != null)
            {
                SerializedProperty toggleValue = property.FindPropertyRelative("enabled");
                if (toggleValue != null)
                {
                    position.height = EditorGUIUtility.singleLineHeight;
                    toggleValue.boolValue = EditorGUI.Toggle(position, toggle.toggleName, toggleValue.boolValue);

                    if (toggleValue.boolValue)
                    {
                        SerializedProperty objectValue = property.FindPropertyRelative("value");
                        if (objectValue != null)
                        {
                            position.y += EditorGUIUtility.singleLineHeight;
                            EditorGUI.PropertyField(position, objectValue, originalLabel, true);
                        }
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty toggleValue = property.FindPropertyRelative("enabled");

            if (toggleValue != null)
            {
                if (toggleValue.boolValue)
                {
                    int childCount = 1;

                    SerializedProperty objectValue = property.FindPropertyRelative("value");
                    if (objectValue.isExpanded)
                    {
                        childCount++;
                        var enumerator = objectValue.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            childCount++;
                        }
                    }

                    return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * childCount;
                }
            }

            return base.GetPropertyHeight(property, label);
        }
    }
}
