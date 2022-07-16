using NoiseGenerator.TerrainGeneration;
using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor;

namespace NoiseGenerator.Editor.TerrainGeneration.Terrain_Generation.Editor
{
	[CustomEditor(typeof(TerrainGenerator))]
	public class TerrainGeneratorEditor : UEditor.Editor
	{
		bool _SettingsFoldout;
		
		public override void OnInspectorGUI()
		{
			var t = target as TerrainGenerator;
			
			EditorGUI.BeginChangeCheck();
			
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Preset"));
			
			if (serializedObject.ApplyModifiedProperties())
				t.Undo();

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("_MeshFilter"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_MeshCollider"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_Material"));

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("_Erosion"));
			serializedObject.FindProperty("_Erode").boolValue =
				EditorGUILayout.ToggleLeft(
					"Erode",
					serializedObject.FindProperty("_Erode").boolValue,
					GUILayout.Width(50)
				);
			
			EditorGUILayout.EndHorizontal();
			
			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.Separator();
			
			EditorGUILayout.BeginHorizontal();
			
			_SettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_SettingsFoldout, "Settings");
			EditorGUILayout.LabelField("Workflow Mode", GUILayout.Width(100));
			t.WorkflowMode =
				(TerrainSettings.WorkflowModes) EditorGUILayout.EnumPopup(t.WorkflowMode, GUILayout.Width(150));
			
			EditorGUILayout.EndHorizontal();


			if (_SettingsFoldout)
			{
				EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                
				EditorGUILayout.Separator();
                
				GUI.contentColor = Color.white * 2f;
				GUI.backgroundColor = new Color(1.4f, 1.4f, 1.4f);

				EditorGUI.indentLevel += 1;

				
				ref var settings = ref t.Settings;

				settings.HeightMultiplier =
					EditorGUILayout.FloatField("Height Multiplier", settings.HeightMultiplier);

				settings.SteepTerrainColor =
					EditorGUILayout.ColorField("Steep Terrain Color", settings.SteepTerrainColor);

				settings.SteepnessThreshold =
					EditorGUILayout.Slider("Steepness Threshold", settings.SteepnessThreshold, 0f, 1f);

				settings.Sharpness =
					EditorGUILayout.Slider("Sharpness", settings.Sharpness, 0f, 1f);
				
				EditorGUILayout.Separator();

				if (t.WorkflowMode == TerrainSettings.WorkflowModes.GradientBased)
					settings.GradientBasedSettings.ColorGradient =
						EditorGUILayout.GradientField("Gradient", settings.GradientBasedSettings.ColorGradient);
				else
				{
					ref var ivSettings = ref settings.IndividualValuesSettings;

					ivSettings.GrassColor =
						EditorGUILayout.ColorField("Grass Color", ivSettings.GrassColor);
					ivSettings.SnowColor =
						EditorGUILayout.ColorField("Snow Color", ivSettings.SnowColor);
					ivSettings.MaxGrassHeight =
						EditorGUILayout.Slider("Max Grass Height", ivSettings.MaxGrassHeight, 0f, 1f);
					ivSettings.MinSnowHeight =
						EditorGUILayout.Slider("Min Snow Height", ivSettings.MinSnowHeight, 0f, 1f);
				}

				
				EditorGUILayout.Separator();

				GUI.backgroundColor = Color.gray * 2f;
 
				EditorGUILayout.EndVertical();

				EditorGUI.indentLevel -= 1;
			}
			
			
			EditorGUILayout.EndFoldoutHeaderGroup();
			
			
			if (EditorGUI.EndChangeCheck())
			{
				if (t.AutoGenerate)
				{
					t.UpdateShader();
					t.UpdateMesh();
				}
				if (t.AutoSave)
					t.Save();
			}

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Generate", GUILayout.MaxWidth(225)))
				t.Generate();
            
			t.AutoGenerate =
				EditorGUILayout.ToggleLeft(
					"Auto Generate", t.AutoGenerate,
					GUILayout.Width(100),
					GUILayout.ExpandWidth(false)
				);
            
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
            
			if (GUILayout.Button("Save Settings", GUILayout.MaxWidth(225)))
				t.Save();
            
			t.AutoSave =
				EditorGUILayout.ToggleLeft(
					"Auto Save", t.AutoSave,
					GUILayout.Width(100),
					GUILayout.ExpandWidth(false)
				);
            
			EditorGUILayout.EndHorizontal();
            
			if (GUILayout.Button("Undo Changes", GUILayout.MaxWidth(225)))
				t.Undo(); 
		}
	}
}