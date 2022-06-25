using NoiseGenerator.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace NoiseGenerator.TerrainGeneration
{
    [InitializeOnLoad]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TerrainGenerator : MonoBehaviour
    {
        public MeshFilter MeshFilter;
        public MeshCollider MeshCollider;
        public bool AutoGenerate;
        public float HeightMultiplier;

        [SerializeField]
        private HeightMapGenerator _HeightMapGenerator;
        [SerializeField]
        private Erosion _Erosion;
        [SerializeField]
        private bool _Erode;
        
        private TerrainShader _Shader;
        private TerrainMeshData _MeshData;
        private float[] _HeightMap;

        private const int _Priority = 5000;

        
        public void GenerateMesh(float[] heightMap = null)
        {
            heightMap ??= _Erode ? _Erosion.Erode(heightMap) 
                                 : _HeightMapGenerator.GenerateHeightMap(_HeightMapGenerator.UseComputeShader);
            
            _HeightMap = heightMap;
            
            _MeshData = new TerrainMeshData(_HeightMapGenerator.NoiseSettings.Size, _HeightMapGenerator.NoiseSettings.Size);
            
            int size = _HeightMapGenerator.NoiseSettings.Size;

            float halfSize  = size * .5f;
            MeshFilter.sharedMesh.indexFormat = IndexFormat.UInt32;

            Helpers.IteratePointsOnMap(size, (x, y, i) => 
            {
                _MeshData.Vertices[i] = new(
                    x - halfSize,
                    heightMap[i] * HeightMultiplier,
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

            MeshFilter.sharedMesh = _MeshData.Get();
            MeshFilter.sharedMesh.RecalculateNormals();
            MeshCollider.sharedMesh = MeshFilter.sharedMesh;

            _Shader.UpdateShader();
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
                _MeshData.Vertices[i].y = _HeightMap[i] * HeightMultiplier;

            // _MeshData.CalculateNormals();
            MeshFilter.sharedMesh = _MeshData.Get();
            MeshFilter.sharedMesh.RecalculateNormals();
            MeshCollider.sharedMesh = MeshFilter.sharedMesh;

            _Shader.UpdateShader();
        }

        private void OnValidate()
        {
            _Shader ??= GetComponent<TerrainShader>();
            
            _HeightMapGenerator.postGenerate.Register(GenerateMesh, _Priority);
        }
    }
}