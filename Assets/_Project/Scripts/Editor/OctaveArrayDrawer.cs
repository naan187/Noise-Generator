using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Editor
{
    [CustomPropertyDrawer(typeof(OctaveArray))]
    public class OctaveArrayDrawer : PropertyDrawer
    {
        //TODO: this
        
        private bool _Foldout;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //var t = (OctaveArray) fieldInfo.GetValue(property);


            Rect foldoutHeaderRect = new(32, position.y, 60, 16);
            _Foldout = EditorGUI.BeginFoldoutHeaderGroup(foldoutHeaderRect, _Foldout, label);

            EditorGUI.IntSlider(position, 2, 1, 8);
            
            EditorGUI.EndFoldoutHeaderGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //if (!_Foldout)
                return base.GetPropertyHeight(property, label);
            
        }
    }
}