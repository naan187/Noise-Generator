using NoiseGenerator.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator.TerrainGeneration
{
    [RequireComponent(typeof(HeightMapGenerator))]
    public class TerrainShader : MonoBehaviour
    {
        [SerializeField] 
        private TerrainPreset _Preset;
        [SerializeField]
        private TerrainShaderSettings ShaderSettings;
        [SerializeField] 
        private Material _Material;


        [SerializeField] 
        private HeightMapGenerator _HeightMapGenerator;

        [SerializeField] 
        private TerrainGenerator _TerrainGenerator;

        public bool AutoUpdate;
        public bool AutoSave;

        private const int _Priority = 4999;

        private static readonly int GradientTexturePropertyId = Shader.PropertyToID("_GradientTexture");
        private static readonly int SteepColorPropertyId = Shader.PropertyToID("_SteepTerrainColor");
        private static readonly int HeightMultiplierPropertyId = Shader.PropertyToID("_HeightMultiplier");
        private static readonly int SteepnessThresholdPropertyId = Shader.PropertyToID("_SteepnessThreshold");
        private static readonly int BlendAmountPropertyId = Shader.PropertyToID("_BlendAmount");

        public void UpdateShader()
        {
            Texture2D gradientTex = new Texture2D(50, 1);

            Color[] texColors = new Color[50];
            for (int i = 0; i < texColors.Length; i++)
                texColors[i] = ShaderSettings.ColorGradient.Evaluate(i / 50f);
            
            gradientTex.SetPixels(texColors);
            gradientTex.wrapMode = TextureWrapMode.Repeat;
            gradientTex.Apply();
            
            _Material.SetTexture(GradientTexturePropertyId, gradientTex);
            _Material.SetColor(SteepColorPropertyId, ShaderSettings.SteepTerrainColor);
            _Material.SetFloat(SteepnessThresholdPropertyId, ShaderSettings.SteepnessThreshold);
            _Material.SetFloat(BlendAmountPropertyId, ShaderSettings.BlendAmount);
            
            _Material.SetFloat(HeightMultiplierPropertyId, _TerrainGenerator.HeightMultiplier);
        }
        
        public void Save() => _Preset.TerrainShaderSettings = ShaderSettings;
        public void Undo() {
            ShaderSettings = _Preset.TerrainShaderSettings;
            UpdateShader();
        }

        private void OnValidate()
        {
            _HeightMapGenerator.postGenerate.Register(UpdateShader, _Priority);
        }
    }
}
