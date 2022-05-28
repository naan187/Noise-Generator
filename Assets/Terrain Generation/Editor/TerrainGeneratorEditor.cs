using UnityEditor;
using UnityEngine;
using NoiseGenerator.TerrainGeneration;

namespace NoiseGenerator.Editor
{
    [CustomEditor(typeof(TerrainGenerator))]
    public class TerrainGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var t = (TerrainGenerator) target;


            if (DrawDefaultInspector())
                if (t.AutoGenerate)
                    t.UpdateMesh();
            
            if (GUILayout.Button("Generate"))
                t.GenerateMesh();
        }
    }
}