using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Editor
{
    [CustomPropertyDrawer(typeof(TYPE))]
    public class OctaveArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}