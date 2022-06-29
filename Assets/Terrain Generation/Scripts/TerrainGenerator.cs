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

        private static readonly int _gradientTexture = Shader.PropertyToID("_GradientTexture");
        private static readonly int _steepTerrainColor = Shader.PropertyToID("_SteepTerrainColor");
        private static readonly int _steepnessThreshold = Shader.PropertyToID("_SteepnessThreshold");
        private static readonly int _sharpness = Shader.PropertyToID("_Sharpness");
        private static readonly int _heightMultiplier = Shader.PropertyToID("_HeightMultiplier");

        private void Start() => Generate();

        public void Generate(float[] heightmap = null)
        {
            GenerateMesh(heightmap);
            UpdateShader();
        }
        
        public void GenerateMesh(float[] heightMap = null)
        {
            heightMap ??= _Erode ? _Erosion.Erode(heightMap) 
                                 : _HeightMapGenerator.GenerateHeightMap(_HeightMapGenerator.UseComputeShader);
            
            _HeightMap = heightMap;
            
            _MeshData = 
                new TerrainMeshData(
                    _HeightMapGenerator.NoiseSettings.Size,
                    _HeightMapGenerator.NoiseSettings.Size
                );
            
            int size = _HeightMapGenerator.NoiseSettings.Size;

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
                    _MeshData.AddTriangle(i, i + size + 1, i + size);
                    _MeshData.AddTriangle(i + size + 1, i, i + 1);
                }
                
                i++;
            });

            _MeshFilter.sharedMesh = _MeshData.Get();
            _MeshFilter.sharedMesh.RecalculateNormals();
            _MeshCollider.sharedMesh = _MeshFilter.sharedMesh;
        }

        public void UpdateMesh()
        {
            if (_HeightMap is null || _MeshData is null)
            {
                GenerateMesh();
                return;
            }
            
            int size = _HeightMapGenerator.NoiseSettings.Size;

            for (int i = 0; i < size * size; i++) 
                _MeshData.Vertices[i].y = _HeightMap[i] * Settings.HeightMultiplier;

            _MeshFilter.sharedMesh = _MeshData.Get();
            _MeshFilter.sharedMesh.RecalculateNormals();
            _MeshCollider.sharedMesh = _MeshFilter.sharedMesh;
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
            
            _Material.SetColor(_steepTerrainColor, Settings.SteepTerrainColor);
            _Material.SetFloat(_steepnessThreshold, Settings.SteepnessThreshold);
            _Material.SetFloat(_sharpness, Settings.Sharpness);
            _Material.SetFloat(_heightMultiplier, Settings.HeightMultiplier);
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

        public void Save() => Preset.TerrainSettings = Settings;
        public void Undo() {
            Settings = Preset.TerrainSettings;
            UpdateShader();
        }

        private void OnEnable()
        {
            _HeightMapGenerator.postGenerate.Register(Generate, 5000);
        }
    }
}