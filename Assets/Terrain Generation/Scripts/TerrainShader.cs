using System;
using NoiseGenerator.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator.TerrainGeneration
{
    [RequireComponent(typeof(HeightMapGenerator))]
    public class TerrainShader : MonoBehaviour
    {
        [FormerlySerializedAs("_WorkflowMode")]
        public TerrainShaderSettings.WorkflowModes WorkflowMode;

        public TerrainPreset Preset;
        
        [FormerlySerializedAs("_ShaderSettings")]
        public TerrainShaderSettings Settings;
        
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
            switch (WorkflowMode)
            {
                case TerrainShaderSettings.WorkflowModes.GradientBased:
                    UpdateShader_GradientBased();
                    break;
                case TerrainShaderSettings.WorkflowModes.IndividualValues:
                    UpdateShader_IndividualValues();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _Material.SetColor(_steepTerrainColor, Settings.SteepTerrainColor);
            _Material.SetFloat(_steepnessThreshold, Settings.SteepnessThreshold);
            _Material.SetFloat(_sharpness, Settings.Sharpness);
            _Material.SetFloat(_heightMultiplier, _TerrainGenerator.HeightMultiplier);
        }

        private void UpdateShader_GradientBased()
        {
            _Material.shader = Shader.Find("Shader Graphs/Terrain_GradientBased");
            
            Texture2D gradientTex = new Texture2D(50, 1);

            Color[] texColors = new Color[50];
            for (int i = 0; i < texColors.Length; i++)
                texColors[i] = Settings.GradientBasedSettings.ColorGradient.Evaluate(i / 50f);

            gradientTex.wrapMode = TextureWrapMode.Repeat;
            gradientTex.SetPixels(texColors);
            gradientTex.Apply();
            
            _Material.SetTexture(_gradientTexture, gradientTex);
        }

        private void UpdateShader_IndividualValues()
        {
            _Material.shader = Shader.Find("Shader Graphs/Terrain_IndividualValues");

            _Material.SetColor("_GrassColor", Settings.IndividualValuesSettings.GrassColor);
            _Material.SetColor("_SnowColor", Settings.IndividualValuesSettings.SnowColor);
            _Material.SetFloat("_MinSnowHeight", Settings.IndividualValuesSettings.MinSnowHeight);
            _Material.SetFloat("_MaxGrassHeight", Settings.IndividualValuesSettings.MaxGrassHeight);
            _Material.SetFloat("_BlendDst", Settings.IndividualValuesSettings.BlendDst);
        }

        public void Save() => Preset.TerrainShaderSettings = Settings;
        public void Undo() {
            Settings = Preset.TerrainShaderSettings;
            UpdateShader();
        }

        private void OnValidate()
        {
            _HeightMapGenerator.postGenerate.Register(UpdateShader, _Priority);
        }
    }
}
