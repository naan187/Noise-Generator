using NoiseGenerator.Core;
using UnityEditor;
using UEditor = UnityEditor;

namespace NoiseGenerator.Editor
{
	[CustomEditor(typeof(NoiseDisplay))]
	public class NoiseDisplayEditor : UEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_HeightMapGenerator"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_TextureRenderer"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_FilterMode"));
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_InvertNoise"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_SampleFromCustomGradient"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_NoiseGradient"));
			serializedObject.ApplyModifiedProperties();
		}
	}
}