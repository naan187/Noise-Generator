using UnityEditor;
using UnityEngine;

namespace NoiseGenerator._Project.Scripts.Editor
{
    ////[CustomPropertyDrawer(typeof(NoiseSettings))]
    //public class NoiseSettingsPropDrawer : PropertyDrawer
    //{
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        //TODO: finish this
//
    //        EditorGUI.BeginProperty(position, label, property);
    //        
    //        
    //        var target = (NoiseSettings) fieldInfo.GetValue(property.serializedObject.targetObject);
//
    //        SerializedProperty mapDimensions = property.FindPropertyRelative("MapDimensions");
    //        SerializedProperty offset = property.FindPropertyRelative("MapDimensions");
    //        SerializedProperty scale = property.FindPropertyRelative("MapDimensions");
//
    //        //target.MapDimensions = EditorGUILayout.Vector2IntField("Map Dimensions", target.MapDimensions);
    //        
    //        //target.Offset = EditorGUILayout.Vector2Field("Offset", target.Offset);
    //        
    //        
    //        GUILayout.Space(15);
    //        
    //        EditorGUI.EndProperty();
    //    }
//
    //    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //    {
    //        return base.GetPropertyHeight(property, label);
    //    }
    //}
}