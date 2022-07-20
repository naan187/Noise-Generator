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
        private float[] _HeightMapBackingField;
        private bool _HeightMapChanged;
        private float[] _HeightMap
        {
            get => _HeightMapBackingField; 
            set
            {
                if (value != _HeightMapBackingField)
                    _HeightMapChanged = true;
                _HeightMapBackingField = value;
            }
        }
        
        [HideInInspector]
        public int MapSize;


        #region property IDs

        private static readonly int _gradientTexture = Shader.PropertyToID("_GradientTexture");
        private static readonly int _steepTerrainColor = Shader.PropertyToID("_SteepTerrainColor");
        private static readonly int _steepnessThreshold = Shader.PropertyToID("_SteepnessThreshold");
        private static readonly int _sharpness = Shader.PropertyToID("_Sharpness");
        private static readonly int _heightMultiplier = Shader.PropertyToID("_HeightMultiplier");
        private static readonly int _grassColor = Shader.PropertyToID("_GrassColor");
        private static readonly int _snowColor = Shader.PropertyToID("_SnowColor");
        private static readonly int _minSnowHeight = Shader.PropertyToID("_MinSnowHeight");
        private static readonly int _maxGrassHeight = Shader.PropertyToID("_MaxGrassHeight");

        #endregion


        private void Start() => Generate();

        public void Generate(float[] heightmap = null)
        {
            _HeightMap = heightmap;
            if (_Erode)
            {
                if (_HeightMapChanged)
                    _HeightMap = _Erosion.Erode(heightmap);
                _HeightMapChanged = false;
                MapSize = _HeightMapGenerator.NoiseSettings.Size - _Erosion.BorderSize;
            }
            else
            {
                GenerateMesh(heightmap);
                MapSize = _HeightMapGenerator.NoiseSettings.Size;
            }

            UpdateShader();
        }

        public void GenerateMesh(float[] heightMap = null)
        {
            heightMap ??= _HeightMapGenerator.GenerateHeightMap(_HeightMapGenerator.UseComputeShader);

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
                    _MeshData.AddTriangle(i, i + size + 1, i + size);
                    _MeshData.AddTriangle(i + size + 1, i, i + 1);
                }
            });

            _MeshFilter.sharedMesh = _MeshData.Get();
            _MeshFilter.sharedMesh.RecalculateNormals();
            _MeshCollider.sharedMesh = _MeshFilter.sharedMesh;
        }

        public void UpdateMesh()
        {
            _HeightMap ??= _HeightMapGenerator.GenerateHeightMap(_HeightMapGenerator.UseComputeShader);
            
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

            _Material.SetColor(_grassColor, Settings.IndividualValuesSettings.GrassColor);
            _Material.SetColor(_snowColor, Settings.IndividualValuesSettings.SnowColor);
            _Material.SetFloat(_minSnowHeight, Settings.IndividualValuesSettings.MinSnowHeight);
            _Material.SetFloat(_maxGrassHeight, Settings.IndividualValuesSettings.MaxGrassHeight);
        }

        public void Save() => Preset.TerrainSettings = new(Settings);
        public void Undo() {
            Settings = new(Preset.TerrainSettings);
            UpdateShader();
        }

        private void OnValidate() => _HeightMapGenerator.postGenerate.Register(Generate, 2147483646);
    }
}