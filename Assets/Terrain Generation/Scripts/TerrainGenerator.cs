using NoiseGenerator.Core;
using UnityEngine;

namespace NoiseGenerator.TerrainGeneration
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TerrainGenerator : MonoBehaviour
    {
        public MeshFilter MeshFilter;
        public bool AutoGenerate;
        public float HeightMultiplier;

        [SerializeField]
        private HeightMapGenerator _HeightMapGenerator;

        private const int _Priority = 5000;


        private TerrainMeshData _MeshData;
        private float[,] _HeightMap;
        
        public void GenerateMesh(float[,] heightMap = null)
        {
            heightMap ??= _HeightMapGenerator.Generate();
            _HeightMap = heightMap;
            
            _MeshData = new TerrainMeshData(_HeightMapGenerator.NoiseSettings.Size, _HeightMapGenerator.NoiseSettings.Size);
            
            int size = _HeightMapGenerator.NoiseSettings.Size;

            float halfSize  = size * .5f;

            int vertexIndex = 0;
            Helpers.IteratePointsOnMap(size, (x, y) => {
                _MeshData.Vertices[vertexIndex] = new(
                    x - halfSize,
                    heightMap[x, y] * HeightMultiplier,
                    y - halfSize
                );
                
                _MeshData.UVs[vertexIndex] = new Vector2(x / (float) size, y / (float) size);

                if (x < size - 1 && y < size - 1)
                {
                    _MeshData.AddTriangle(vertexIndex, vertexIndex + size + 1, vertexIndex + size);
                    _MeshData.AddTriangle(vertexIndex + size + 1, vertexIndex, vertexIndex + 1);
                }
                
                vertexIndex++;
            });
            
            MeshFilter.sharedMesh = _MeshData.Get();
            MeshFilter.sharedMesh.RecalculateNormals();
        }

        public void UpdateMesh()
        {
            if (_HeightMap is null)
            {
                GenerateMesh();
                return;
            }
            
            int size = _HeightMapGenerator.NoiseSettings.Size;

            int vertexIndex = 0;
            Helpers.IteratePointsOnMap(size, (x, y) =>
            {
                _MeshData.Vertices[vertexIndex].y = _HeightMap[x, y] * HeightMultiplier;
                vertexIndex++;
            });

            MeshFilter.sharedMesh = _MeshData.Get();
        }

        private void OnValidate()
        {
            _HeightMapGenerator.postGenerate.Register(GenerateMesh, _Priority);
        }
    }
}