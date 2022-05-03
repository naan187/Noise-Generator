using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Editor
{
    [CustomEditor(typeof(HeightMapGenerator))]
    public class NoiseMapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var t = (HeightMapGenerator) target;

            if (GUILayout.Button("Generate"))
                t.Generate();
            if (GUILayout.Button("Save to Preset"))
                t.Save();
            if (GUILayout.Button("Undo Changes"))
                t.Undo();
        }
    }
}