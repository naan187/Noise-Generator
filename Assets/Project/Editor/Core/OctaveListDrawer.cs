using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Editor
{
    //[CustomPropertyDrawer(typeof(OctaveList))]
    public class OctaveListDrawer : PropertyDrawer
    {
        private bool _Foldout;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enumerator = property.FindPropertyRelative("_Octaves");
            
            EditorGUI.PropertyField(
                new Rect(16, position.y, position.width, EditorGUI.GetPropertyHeight(enumerator)),
                enumerator
            );

            
            var amountSliderRect = new Rect(
                position.center.x, position.y, position.width * .5f,
                EditorGUI.GetPropertyHeight(property)
            );
            
            int octaveAmount = property.FindPropertyRelative("OctaveAmount").intValue;

            octaveAmount = EditorGUI.IntSlider(
                amountSliderRect,
                octaveAmount, 1, 8
            );

            property.FindPropertyRelative("OctaveAmount").intValue = octaveAmount;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enumeratorProp = property.FindPropertyRelative("_Octaves");
            return EditorGUI.GetPropertyHeight(enumeratorProp);
        }
    }
}