using UnityEditor;
using UnityEngine;

namespace NoiseGenerator.Editor
{
    [CustomEditor(typeof(HeightMapGenerator))]
    public class HeightMapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var t = (HeightMapGenerator) target;
            
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