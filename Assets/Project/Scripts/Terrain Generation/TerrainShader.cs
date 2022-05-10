using System;
using NoiseGenerator.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator.TerrainGeneration
{
    public class TerrainShader : MonoBehaviour
    {
        [SerializeField] private Material _Material;
        [SerializeField] private Gradient _HeightBasedTerrainGradient;
        [SerializeField] private Color _SteepTerrainColor;
        [SerializeField] private float _BlendThreshold;
        
        [SerializeField] private HeightMapGeneratorMono _HeightMapGenerator;
        [SerializeField] private TerrainGenerator _TerrainGenerator;

        public bool AutoUpdate;

        private static readonly int GradientTexturePropertyId = Shader.PropertyToID("_GradientTexture");
        private static readonly int SteepColorPropertyId = Shader.PropertyToID("_SteepTerrainColor");
        private static readonly int HeightMultiplierPropertyId = Shader.PropertyToID("_HeightMultiplier");
        private static readonly int BlendThresholdPropertyId = Shader.PropertyToID("_BlendThreshold");


        public void UpdateShader()
        {
            Texture2D gradientTex = new Texture2D(50, 1);

            Color[] texColors = new Color[50];
            for (int i = 0; i < texColors.Length; i++)
                texColors[i] = _HeightBasedTerrainGradient.Evaluate(i / 50f);
            
            gradientTex.SetPixels(texColors);
            gradientTex.Apply();
            
            _Material.SetTexture(GradientTexturePropertyId, gradientTex);
            _Material.SetColor(SteepColorPropertyId, _SteepTerrainColor);
            _Material.SetFloat(BlendThresholdPropertyId, _BlendThreshold);
            _Material.SetFloat(HeightMultiplierPropertyId, _TerrainGenerator.HeightMultiplier);
        }

        private void OnValidate() => _HeightMapGenerator.PostGenerate += UpdateShader;
    }
}
