using UnityEditor;
using UnityEngine;
using NoiseGenerator.Core;

namespace NoiseGenerator.Editor
{
    [CustomEditor(typeof(HeightMapGeneratorMono))]
    public class HeightMapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var t = target as NoiseGenerator.Core.HeightMapGeneratorMono;

            if (DrawDefaultInspector())
            {
                if (t.AutoGenerate)
                    t.Generate();
                if (t.AutoSave)
                    t.Save();
            }

            if (GUILayout.Button("Generate"))
                t.Generate();
            if (GUILayout.Button("Save to Preset"))
                t.Save();
            if (GUILayout.Button("Undo Changes"))
                t.Undo();
        }
    }
}