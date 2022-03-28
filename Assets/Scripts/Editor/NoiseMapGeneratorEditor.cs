using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseMapGenerator))]
public class NoiseMapGeneratorEditor : Editor
{
    private NoiseMapGenerator.OnGenerate _onGenerate = NoiseMapGenerator.onGenerate;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _onGenerate ??= NoiseMapGenerator.onGenerate;
        if (GUILayout.Button("Generate")) _onGenerate?.Invoke();
    }
}