using UEditor = UnityEditor;
using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Core.Editor
{
    [CustomEditor(typeof(HeightMapGenerator))]
    public class HeightMapGeneratorEditor : UEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var t = target as HeightMapGenerator;

            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Preset"));
            serializedObject.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck())
                t.Undo();


            _SettingsFoldout = 
                EditorGUILayout.BeginFoldoutHeaderGroup(_SettingsFoldout, "Settings");
            
            if (_SettingsFoldout)
            {
                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                
                EditorGUILayout.Separator();
                
                GUI.contentColor = Color.white * 2f;
                GUI.backgroundColor = new Color(1.4f, 1.4f, 1.4f);

                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Seed", GUILayout.Width(100));
                
                t.RandomizeSeed =
                    EditorGUILayout.ToggleLeft(
                        "Randomize", t.RandomizeSeed,
                        GUILayout.Width(100)
                    );
                
                t.NoiseSettings.Seed =
                    EditorGUILayout.IntField(
                        t.NoiseSettings.Seed,
                        GUILayout.Width(75),
                        GUILayout.ExpandWidth(true)
                    );
                 
                EditorGUILayout.EndHorizontal();

                
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

                GUI.backgroundColor = Color.gray * 2f;
 
                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel -= 1;
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
            
 
            t.UseComputeShader = EditorGUILayout.Toggle("Use Compute Shader", t.UseComputeShader);

            if (EditorGUI.EndChangeCheck())
            {
                if (t.AutoGenerate)
                    t.Generate();
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

        private bool _SettingsFoldout = true;
    }
}