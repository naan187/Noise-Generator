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

        private const int _Priority = 4999;

        public void UpdateShader()
        {
            Texture2D gradientTex = new Texture2D(50, 1);

            Color[] texColors = new Color[50];
            for (int i = 0; i < texColors.Length; i++)
                texColors[i] = _ShaderSettings.ColorGradient.Evaluate(i / 50f);
            
            gradientTex.SetPixels(texColors);
            gradientTex.wrapMode = TextureWrapMode.Repeat;
            gradientTex.Apply();
            
            _Material.SetTexture("_GradientTexture", gradientTex);
            _Material.SetColor("_SteepTerrainColor", _ShaderSettings.SteepTerrainColor);
            _Material.SetFloat("_SteepnessThreshold", _ShaderSettings.SteepnessThreshold);
            _Material.SetFloat("_Sharpness", _ShaderSettings.Sharpness);
            _Material.SetFloat("_HeightMultiplier", _TerrainGenerator.HeightMultiplier);
            _Material.SetColor("_WaterColor", _ShaderSettings.WaterColor);
            _Material.SetFloat("_WaterLevel", _ShaderSettings.Waterlevel);
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
    }
}
