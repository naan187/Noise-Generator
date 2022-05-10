using System;
using NoiseGenerator.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator.TerrainGeneration
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TerrainGenerator : MonoBehaviour
    {
        public MeshFilter MeshFilter;
        public bool AutoGenerate;
        [FormerlySerializedAs("_HeightMultiplier")] public float HeightMultiplier;

        [SerializeField]
        private HeightMapGeneratorMono _HeightMapGenerator;

        
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
                _MeshData.Vertices[vertexIndex] = new Vector3(x - halfSize, heightMap[x, y], y - halfSize);
                _MeshData.UVs[vertexIndex] = new Vector2(x / (float) size, y / (float) size);

                if (x < size - 1 && y < size - 1)
                {
                    _MeshData.AddTriangle(vertexIndex, vertexIndex + size + 1, vertexIndex + size);
                    _MeshData.AddTriangle(vertexIndex + size + 1, vertexIndex, vertexIndex + 1);
                }
                
                vertexIndex++;
            });
            
            SetMeshScaled(size);
            MeshFilter.transform.localScale = new Vector3(size * .1f, 1, size * .1f);
        }

        private void SetMeshScaled(int size)
        {
            int i = 0;
            Helpers.IteratePointsOnMap(size, (x, y) => {
                _MeshData.Vertices[i].y *= HeightMultiplier;
                i++;
            });
            
            MeshFilter.sharedMesh = _MeshData.Get();
            MeshFilter.sharedMesh.RecalculateNormals();

            i = 0;
            Helpers.IteratePointsOnMap(size, (x, y) => {
                _MeshData.Vertices[i].y /= HeightMultiplier;
                i++;
            });
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

        private void OnValidate() => _HeightMapGenerator.PostGenerate_WithHeightmap += GenerateMesh;
    }
}