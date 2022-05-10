using NoiseGenerator.TerrainGeneration;
using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Editor
{
	[CustomEditor(typeof(TerrainShader))]
	public class TerrainShaderEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var t = (TerrainShader) target;
			
			if (DrawDefaultInspector())
				if (t.AutoUpdate)
					t.UpdateShader();
			
			if (GUILayout.Button("Update Shader"))
				t.UpdateShader();
		}
	}
}