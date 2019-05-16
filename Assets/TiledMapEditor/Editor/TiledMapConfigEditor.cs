using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEditorInternal;

namespace AillieoUtils.TiledMapEditor
{

    [CustomPropertyDrawer(typeof(ConfigItem))]
    public class ConfigItemDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rectLeft = new Rect(position.x, position.y, position.width / 2, position.height);
            var rectMiddle = new Rect(position.x + position.width / 2, position.y, position.width / 4, position.height);
            var rectRight = new Rect(position.x + position.width * 3 / 4, position.y, position.width / 4, position.height);

            EditorGUI.PropertyField(rectLeft, property.FindPropertyRelative("displayName"), GUIContent.none);
            EditorGUI.PropertyField(rectMiddle, property.FindPropertyRelative("brushValue"), GUIContent.none);
            EditorGUI.PropertyField(rectRight, property.FindPropertyRelative("displayColor"), GUIContent.none);
        }
    }


    [CustomEditor(typeof(TiledMapConfig))]
    public class CityGridsColorConfigEditor : Editor
    {
        ReorderableList reorderableList;
        SerializedProperty items;

        private void OnEnable()
        {
            items = serializedObject.FindProperty("Items");
            reorderableList = new ReorderableList(serializedObject, items);
            reorderableList.drawHeaderCallback += rect => GUI.Label(rect, "items");
            reorderableList.elementHeight = EditorGUIUtility.singleLineHeight;
            reorderableList.drawElementCallback += (rect, index, isActive, isFocused) => {
                var oneConfig = items.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect,oneConfig);
            };
        }

        public override void OnInspectorGUI()
        {
            reorderableList.DoLayoutList();
        }
    }
}

