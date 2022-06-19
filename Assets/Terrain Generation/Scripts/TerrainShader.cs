using System;
using NoiseGenerator.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator.TerrainGeneration
{
    [RequireComponent(typeof(HeightMapGenerator))]
    public class TerrainShader : MonoBehaviour
    {
        [SerializeField]
        private WorkflowMode _WorkflowMode;

        [SerializeField] 
        private TerrainPreset _Preset;
        [FormerlySerializedAs("ShaderSettings")] [SerializeField]
        private TerrainShaderSettings _ShaderSettings;
        [SerializeField] 
        private Material _Material;
        [SerializeField] 
        private HeightMapGenerator _HeightMapGenerator;
        [SerializeField] 
        private TerrainGenerator _TerrainGenerator;

        public bool AutoUpdate;
        public bool AutoSave;
        private static readonly int _gradientTexture = Shader.PropertyToID("_GradientTexture");
        private static readonly int _steepTerrainColor = Shader.PropertyToID("_SteepTerrainColor");
        private static readonly int _steepnessThreshold = Shader.PropertyToID("_SteepnessThreshold");
        private static readonly int _sharpness = Shader.PropertyToID("_Sharpness");
        private static readonly int _heightMultiplier = Shader.PropertyToID("_HeightMultiplier");

        private const int _Priority = 4997;

        private void Start() => UpdateShader();

        public void UpdateShader()
        {
            switch (_WorkflowMode)
            {
                case WorkflowMode.GradientBased:
                    UpdateShader_GradientBased();
                    break;
                case WorkflowMode.FloatValues:
                    UpdateShader_FloatValues();
                    break;
            }
        }

        private void UpdateShader_GradientBased()
        {
            Texture2D gradientTex = new Texture2D(50, 1);

            Color[] texColors = new Color[50];
            for (int i = 0; i < texColors.Length; i++)
                texColors[i] = _ShaderSettings.ColorGradient.Evaluate(i / 50f);

            gradientTex.wrapMode = TextureWrapMode.Repeat;
            gradientTex.SetPixels(texColors);
            gradientTex.Apply();
            
            _Material.SetTexture(_gradientTexture, gradientTex);
            _Material.SetColor(_steepTerrainColor, _ShaderSettings.SteepTerrainColor);
            _Material.SetFloat(_steepnessThreshold, _ShaderSettings.SteepnessThreshold);
            _Material.SetFloat(_sharpness, _ShaderSettings.Sharpness);
            _Material.SetFloat(_heightMultiplier, _TerrainGenerator.HeightMultiplier);
        }

        private void UpdateShader_FloatValues()
        {
            
        }

        public void Save() => _Preset.TerrainShaderSettings = _ShaderSettings;
        public void Undo() {
            _ShaderSettings = _Preset.TerrainShaderSettings;
            UpdateShader();
        }

        private void OnValidate()
        {
            _HeightMapGenerator.postGenerate.Register(UpdateShader, _Priority);
        }

        enum WorkflowMode
        {
            GradientBased = 0,
            FloatValues = 1
        }
    }
}
