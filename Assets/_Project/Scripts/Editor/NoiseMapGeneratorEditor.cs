using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Editor
{
    [CustomEditor(typeof(NoiseMapGenerator))]
    public class NoiseMapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var t = (NoiseMapGenerator) target;

            if (GUILayout.Button("Generate"))
                t.Generate();
            if (GUILayout.Button("Save to Preset"))
                t.Save();
            if (GUILayout.Button("Revert"))
                t.Undo();
        }
    }
}