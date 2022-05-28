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
			{
				if (t.AutoUpdate)
					t.UpdateShader();
				if (t.AutoSave)
					t.Save();
			}

			if (GUILayout.Button("Update Shader"))
				t.UpdateShader();
			if (GUILayout.Button("Save Settings"))
				t.Save();
			if (GUILayout.Button("Undo Changes"))
				t.Undo();
		}
	}
}