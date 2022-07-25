using NoiseGenerator.Core;
using UnityEngine;
using UnityEngine.Rendering;
namespace NoiseGenerator.TerrainGeneration
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TerrainGenerator : MonoBehaviour
    {
        public TerrainSettings.WorkflowModes WorkflowMode;
        
        [SerializeField]
        private MeshFilter _MeshFilter;
        [SerializeField]
        private MeshCollider _MeshCollider;
        public TerrainPreset Preset;
        public TerrainSettings Settings;
        public bool AutoGenerate;
        public bool AutoSave;
        
        [SerializeField]
        private HeightMapGenerator _HeightMapGenerator;
        [SerializeField]
        private Erosion _Erosion;
        [SerializeField]
        private bool _Erode;
        [SerializeField]
        private Material _Material;


        private TerrainMeshData _MeshData;
        private float[] _HeightMap;


        #region property IDs
        
        private static readonly int _GradientTexture = Shader.PropertyToID("_GradientTexture");
        private static readonly int _SteepTerrainColor = Shader.PropertyToID("_SteepTerrainColor");
        private static readonly int _SteepnessThreshold = Shader.PropertyToID("_SteepnessThreshold");
        private static readonly int _Sharpness = Shader.PropertyToID("_Sharpness");
        private static readonly int _HeightMultiplier = Shader.PropertyToID("_HeightMultiplier");
        private static readonly int _GrassColor = Shader.PropertyToID("_GrassColor");
        private static readonly int _SnowColor = Shader.PropertyToID("_SnowColor");
        private static readonly int _MinSnowHeight = Shader.PropertyToID("_MinSnowHeight");
        private static readonly int _MaxGrassHeight = Shader.PropertyToID("_MaxGrassHeight");
        
        #endregion
        
        
        private void Start() => Generate();

        public void Generate(float[] heightmap = null)
        {
            if (_Erode)
                _Erosion.Erode(heightmap);
            else
                GenerateMesh(heightmap);
            UpdateShader();
        }

        public void GenerateMesh(float[] heightMap = null)
        {
            heightMap ??= _HeightMapGenerator.GenerateHeightMap();
            _HeightMap = heightMap;
            int size = _HeightMapGenerator.NoiseSettings.Size;
            
            _MeshData = new TerrainMeshData(size);
            float halfSize  = size * .5f;
            _MeshFilter.sharedMesh.indexFormat = IndexFormat.UInt32;
            Helpers.IteratePointsOnMap(size, (x, y, i) => 
            {
                _MeshData.Vertices[i] = new(
                    x - halfSize,
                    heightMap[i] * Settings.HeightMultiplier,
                    y - halfSize
                );
                
                _MeshData.UVs[i] = new Vector2(x / (float) size, y / (float) size);
                if (x < size - 1 && y < size - 1)
                {
                    _MeshData.AddTriangle(i + size, i + size + 1, i);
                    _MeshData.AddTriangle(i + 1, i, i + size + 1);
                }
            });

            _MeshFilter.sharedMesh = _MeshData.Get();
            _MeshFilter.sharedMesh.RecalculateNormals();
            _MeshCollider.sharedMesh = _MeshData.Get();
        }

        public void UpdateMesh()
        {
            _HeightMap ??= _Erode
                ? _Erosion.Erode()
                : _HeightMapGenerator.GenerateHeightMap();

            ref var size = ref _HeightMapGenerator.NoiseSettings.Size;

            _MeshData ??= new(size);
            for (int i = 0; i < size * size; i++) 
                _MeshData.Vertices[i].y = _HeightMap[i] * Settings.HeightMultiplier;

            _MeshFilter.sharedMesh = _MeshData.Get();
            _MeshFilter.sharedMesh.RecalculateNormals();
            _MeshCollider.sharedMesh = _MeshData.Get();
        }

        public void UpdateShader()
        {
            switch (WorkflowMode)
            {
                case TerrainSettings.WorkflowModes.GradientBased:
                    UpdateShader_GradientBased();
                    break;
                case TerrainSettings.WorkflowModes.IndividualValues:
                    UpdateShader_IndividualValues();
                    break;
            }
            
            _Material.SetColor(_SteepTerrainColor, Settings.SteepTerrainColor);
            _Material.SetFloat(_SteepnessThreshold, Settings.SteepnessThreshold);
            _Material.SetFloat(_Sharpness, Settings.Sharpness);
            _Material.SetFloat(_HeightMultiplier, Settings.HeightMultiplier);
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
            
            _Material.SetTexture(_GradientTexture, gradientTex);
        }
        private void UpdateShader_IndividualValues()
        {
            _Material.shader = Shader.Find("Shader Graphs/Terrain_IndividualValues");
            _Material.SetColor(_GrassColor, Settings.IndividualValuesSettings.GrassColor);
            _Material.SetColor(_SnowColor, Settings.IndividualValuesSettings.SnowColor);
            _Material.SetFloat(_MinSnowHeight, Settings.IndividualValuesSettings.MinSnowHeight);
            _Material.SetFloat(_MaxGrassHeight, Settings.IndividualValuesSettings.MaxGrassHeight);
        }
        public void Save() => Preset.TerrainSettings = new(Settings);
        public void Undo() {
            Settings = new(Preset.TerrainSettings);
            UpdateShader();
        }

        private void OnValidate() => _HeightMapGenerator.postGenerate.Register(Generate, 2147483646);
    }
}