using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Editors
{
    [CustomEditor(typeof(NoiseMapGenerator))]
    public class NoiseMapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate")) NoiseMapGenerator.OnGenerate?.Invoke();
        }
    }
}