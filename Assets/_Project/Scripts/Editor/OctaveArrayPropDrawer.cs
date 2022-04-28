using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Editor
{
    [CustomPropertyDrawer(typeof(OctaveArray))]
    public class OctaveArrayPropDrawer : PropertyDrawer
    {
        private bool _foldout = true;

        //TODO: finish this
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // var target = (OctaveArray) fieldInfo.GetValue(property.serializedObject.targetObject);

            EditorGUI.BeginProperty(position, label, property);

            var octaves = property.FindPropertyRelative("_Octaves");
            var editOctaves = property.FindPropertyRelative("_EditOctaves");

            Rect foldoutButtonRect = new Rect(position.x, position.y, 16, 16);

            _foldout = EditorGUI.Foldout(foldoutButtonRect, _foldout, label);

            if (!_foldout)
            {
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.indentLevel += 1;

            // EditorGUI.LabelField(new Rect(EditorGUI.indentLevel, 32 + position.y, 60, 16), "Test");

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}
