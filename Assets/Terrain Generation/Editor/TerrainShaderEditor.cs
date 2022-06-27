using System;
using NoiseGenerator.TerrainGeneration;
using UnityEditor;
using UnityEngine;
using static NoiseGenerator.TerrainGeneration.TerrainShaderSettings;

namespace NoiseGenerator.Editor
{
	[CustomEditor(typeof(TerrainShader))]
	public class TerrainShaderEditor : UnityEditor.Editor
	{
		bool _SettingsFoldout;
		
		public override void OnInspectorGUI()
		{
			var t = (TerrainShader) target;
			
			EditorGUI.BeginChangeCheck();
			
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Preset"));
			
			if (serializedObject.ApplyModifiedProperties())
				t.Undo();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("_Material"));
			serializedObject.ApplyModifiedProperties();

			
			EditorGUILayout.BeginHorizontal();
			
			_SettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_SettingsFoldout, "Settings");
			EditorGUILayout.LabelField("Workflow Mode", GUILayout.Width(100));
			t.WorkflowMode =
				(WorkflowModes) EditorGUILayout.EnumPopup(t.WorkflowMode, GUILayout.Width(150));
			
			EditorGUILayout.EndHorizontal();


			if (_SettingsFoldout)
			{
				EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                
				EditorGUILayout.Separator();
                
				GUI.contentColor = Color.white * 2f;
				GUI.backgroundColor = new Color(1.4f, 1.4f, 1.4f);

				EditorGUI.indentLevel += 1;

				
				var settings = t.Settings;

				settings.SteepTerrainColor =
					EditorGUILayout.ColorField("Steep Terrain Color", settings.SteepTerrainColor);

				settings.SteepnessThreshold =
					EditorGUILayout.Slider("Steepness Threshold", settings.SteepnessThreshold, 0f, 1f);

				settings.Sharpness =
					EditorGUILayout.Slider("Steepness Threshold", settings.Sharpness, 0f, 1f);
				
				EditorGUILayout.Separator();

				if (t.WorkflowMode == WorkflowModes.GradientBased)
					settings.GradientBasedSettings.ColorGradient =
						EditorGUILayout.GradientField("Gradient", settings.GradientBasedSettings.ColorGradient);
				else
				{
					ref var ivSettings = ref settings.IndividualValuesSettings;

					ivSettings.GrassColor =
						EditorGUILayout.ColorField("Grass Color", ivSettings.GrassColor);
					ivSettings.SnowColor =
						EditorGUILayout.ColorField("Grass Color", ivSettings.SnowColor);
					ivSettings.MaxGrassHeight =
						EditorGUILayout.Slider("Max Grass Height", ivSettings.MaxGrassHeight, 0f, 1f);
					ivSettings.MinSnowHeight =
						EditorGUILayout.Slider("Max Grass Height", ivSettings.MinSnowHeight, 0f, 1f);
					ivSettings.BlendDst = 
						EditorGUILayout.Slider("Max Grass Height", ivSettings.BlendDst, 0f, 1f);
				}

				
				EditorGUILayout.Separator();

				GUI.backgroundColor = Color.gray * 2f;
				GUI.backgroundColor = Color.gray * 2f;
 
				EditorGUILayout.EndVertical();

				EditorGUI.indentLevel -= 1;
			}
			
			
			EditorGUILayout.EndFoldoutHeaderGroup();
			
			
			if (EditorGUI.EndChangeCheck())
			{
				if (t.AutoUpdate)
					t.UpdateShader();
				if (t.AutoSave)
					t.Save();
			}

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Generate", GUILayout.MaxWidth(225)))
				t.UpdateShader();
            
			t.AutoUpdate =
				EditorGUILayout.ToggleLeft(
					"Auto Update", t.AutoUpdate,
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