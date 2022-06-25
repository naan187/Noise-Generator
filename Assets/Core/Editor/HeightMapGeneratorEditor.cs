using UEditor = UnityEditor;
using UnityEditor;
using UnityEngine;
using NoiseGenerator.Core;

namespace NoiseGenerator.Editor
{
    [CustomEditor(typeof(HeightMapGenerator))]
    public class HeightMapGeneratorEditor : UEditor.Editor
    {
        private bool _SettingsFoldout = true;
        
        public override void OnInspectorGUI()
        {
            var t = target as HeightMapGenerator;

            EditorGUI.BeginChangeCheck();


            EditorGUILayout.PropertyField(serializedObject.FindProperty("Preset"));

            t.UseComputeShader = EditorGUILayout.Toggle("Use Compute Shader", t.UseComputeShader);

            _SettingsFoldout = 
                EditorGUILayout.BeginFoldoutHeaderGroup(_SettingsFoldout, "Noise Settings");
            
            if (_SettingsFoldout)
            {
                EditorGUILayout.BeginVertical(EditorStyles.objectField);
                
                EditorGUILayout.Separator();
                
                GUI.contentColor = Color.white * 2f;
                GUI.backgroundColor = new Color(1.4f, 1.4f, 1.4f);

                var settings = t.NoiseSettings;
                
                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Seed", GUILayout.Width(100));
                
                t.RandomizeSeed =
                    EditorGUILayout.ToggleLeft(
                        "Randomize", t.RandomizeSeed,
                        GUILayout.Width(100)
                    );
                
                settings.Seed =
                    EditorGUILayout.IntField(settings.Seed, GUILayout.Width(75), GUILayout.ExpandWidth(true));
                
                EditorGUILayout.EndHorizontal();

                
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Map Size", GUILayout.Width(85));
                settings.Size = EditorGUILayout.IntSlider(settings.Size, 16, 256);
                
                EditorGUILayout.EndHorizontal();

                
                settings.Offset = EditorGUILayout.Vector2Field("Offset", settings.Offset, GUILayout.ExpandHeight(false));
                
                EditorGUILayout.Separator();

                settings.Scale = EditorGUILayout.FloatField("Noise Scale", settings.Scale);

                
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Octave Amount");
                settings.OctaveAmount = EditorGUILayout.IntSlider(settings.OctaveAmount, 1, 8);
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Separator();

                settings.Persistence = EditorGUILayout.Slider("Persistence", settings.Persistence, 0f, 1f);
                
                settings.Lacunarity = EditorGUILayout.FloatField("Lacunarity", settings.Lacunarity);

                settings.HeightCurve = EditorGUILayout.CurveField("HeightCurve", settings.HeightCurve);

                EditorGUILayout.Separator();

                GUI.backgroundColor = Color.gray * 2f;
                GUI.backgroundColor = Color.gray * 2f;

                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel -= 1;
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();

            
            if (EditorGUI.EndChangeCheck())
            {
                if (t.AutoGenerate)
                    t.Generate(t.UseComputeShader);
                if (t.AutoSave)
                    t.Save();
            }

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Generate", GUILayout.MaxWidth(225)))
                t.Generate(t.UseComputeShader);
            
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