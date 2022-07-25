using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.TerrainGeneration.Editor
{
	[CustomEditor(typeof(TerrainPreset))]
	public class TerrainPresetEditor : UnityEditor.Editor
	{
        private bool _NoiseSettingsFoldout;
        private bool _ShaderSettingsFoldout;

        public override void OnInspectorGUI()
		{
			var t = target as TerrainPreset;
			
			_NoiseSettingsFoldout = 
                EditorGUILayout.BeginFoldoutHeaderGroup(_NoiseSettingsFoldout, "Noise Settings");
            
            if (_NoiseSettingsFoldout)
            {
                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                
                EditorGUILayout.Separator();
            
                GUI.contentColor = Color.white * 2f;
                GUI.backgroundColor = new Color(1.4f, 1.4f, 1.4f);

                EditorGUI.indentLevel += 1;

                t.NoiseSettings.Seed =
                    EditorGUILayout.IntField(
                        "Seed",
                        t.NoiseSettings.Seed,
                        GUILayout.Width(75),
                        GUILayout.ExpandWidth(true)
                    );
             

                t.NoiseSettings.Size =
                    EditorGUILayout.IntSlider("Map Size", t.NoiseSettings.Size, 16, 256);
            
                t.NoiseSettings.Offset =
                    EditorGUILayout.Vector2Field("Offset", t.NoiseSettings.Offset, GUILayout.ExpandHeight(false));
            
                EditorGUILayout.Separator();

                t.NoiseSettings.Scale = EditorGUILayout.FloatField("Noise Scale", t.NoiseSettings.Scale);
                if (t.NoiseSettings.Scale <= .75f)
                    t.NoiseSettings.Scale = .75f;

            
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Octave Amount");
                t.NoiseSettings.OctaveAmount =
                    EditorGUILayout.IntSlider(t.NoiseSettings.OctaveAmount, 1, 8);
            
                EditorGUILayout.EndHorizontal();
            
                EditorGUILayout.Separator();

                t.NoiseSettings.Persistence =
                    EditorGUILayout.Slider("Persistence", t.NoiseSettings.Persistence, 0f, 1f);
            
                t.NoiseSettings.Lacunarity =
                    EditorGUILayout.FloatField("Lacunarity", t.NoiseSettings.Lacunarity);

                t.NoiseSettings.HeightCurve =
                    EditorGUILayout.CurveField("HeightCurve", t.NoiseSettings.HeightCurve);

                EditorGUILayout.Separator();

                EditorGUI.indentLevel -= 1;
                
                GUI.contentColor = Color.gray * 2f;
                GUI.backgroundColor = Color.gray * 2f;

                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
            

            _ShaderSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_ShaderSettingsFoldout, "Shader Settings");

            if (_ShaderSettingsFoldout)
            {
                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                
                EditorGUILayout.Separator();
                
                GUI.contentColor = Color.white * 2f;
                GUI.backgroundColor = new Color(1.4f, 1.4f, 1.4f);

                EditorGUI.indentLevel += 1;

				
                ref var settings = ref t.TerrainSettings;

                settings.HeightMultiplier =
                    EditorGUILayout.FloatField("Height Multiplier", settings.HeightMultiplier);
				
                settings.SteepTerrainColor =
                    EditorGUILayout.ColorField("Steep Terrain Color", settings.SteepTerrainColor);

                settings.SteepnessThreshold =
                    EditorGUILayout.Slider("Steepness Threshold", settings.SteepnessThreshold, 0f, 1f);

                settings.Sharpness =
                    EditorGUILayout.Slider("Sharpness", settings.Sharpness, 0f, 1f);
				
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                
                settings.GradientBasedSettings.ColorGradient =
                    EditorGUILayout.GradientField("Gradient", settings.GradientBasedSettings.ColorGradient);
                
                EditorGUILayout.Separator();
                
                ref var ivSettings = ref settings.IndividualValuesSettings;

                ivSettings.GrassColor =
                    EditorGUILayout.ColorField("Grass Color", ivSettings.GrassColor);
                ivSettings.SnowColor =
                    EditorGUILayout.ColorField("Snow Color", ivSettings.SnowColor);
                ivSettings.MaxGrassHeight =
                    EditorGUILayout.Slider("Max Grass Height", ivSettings.MaxGrassHeight, 0f, 1f);
                ivSettings.MinSnowHeight =
                    EditorGUILayout.Slider("Min Snow Height", ivSettings.MinSnowHeight, 0f, 1f);

				
                EditorGUILayout.Separator();

                GUI.backgroundColor = Color.gray * 2f;
 
                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel -= 1;
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
		}
	}
}