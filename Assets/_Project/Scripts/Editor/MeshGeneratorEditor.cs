using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Editor
{
    [CustomEditor(typeof(MeshGenerator))]
    public class MeshGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = (MeshGenerator) target;

            if (GUILayout.Button("Generate"))
                t.GenerateMesh();
        }
    }
}