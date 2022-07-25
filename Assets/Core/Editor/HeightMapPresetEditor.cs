using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Core.Editor
{
	[CustomEditor(typeof(HeightmapPreset))]
	public class HeightMapPresetEditor : UnityEditor.Editor
	{
        private bool _SettingsFoldout;

        public override void OnInspectorGUI()
		{
			var t = target as HeightmapPreset;

            _SettingsFoldout = 
                EditorGUILayout.BeginFoldoutHeaderGroup(_SettingsFoldout, "Settings");
            
            if (_SettingsFoldout)
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
                
                GUI.backgroundColor = Color.gray * 2f;

                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
		}
	}
}