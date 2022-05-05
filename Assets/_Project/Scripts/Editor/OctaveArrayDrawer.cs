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
            Rect foldoutRect = new Rect(position.x, position.y, position.width * .5f, position.height);
            _Foldout = EditorGUI.Foldout(foldoutRect, _Foldout, label, true);

            int octaveAmount = property.FindPropertyRelative("OctaveAmount").intValue;

            octaveAmount = EditorGUI.IntSlider(
                new Rect(position.width * .5f, position.y, position.width * .5f, 16),
                octaveAmount, 1, 8
            );

            property.FindPropertyRelative("OctaveAmount").intValue = octaveAmount;

            if (!_Foldout)
                return;


        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //if (!_Foldout)
                return base.GetPropertyHeight(property, label);
            
        }
    }
}