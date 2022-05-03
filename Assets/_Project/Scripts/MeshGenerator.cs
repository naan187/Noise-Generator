using UnityEngine;

namespace NoiseGenerator
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshGenerator : MonoBehaviour
    {
        public MeshFilter MeshFilter;

        [SerializeField] 
        private float _HeightMultiplier;
        [SerializeField]
        private Transform _Transform;

        private HeightMapGenerator _HeightMapGenerator;
        

        public MeshData GenerateMesh(float[,] heightMap = null)
        {
            _HeightMapGenerator ??= _Transform.GetComponent<HeightMapGenerator>();
            
            heightMap ??= _HeightMapGenerator.Generate();
            
            MeshData meshData = new MeshData(_HeightMapGenerator.NoiseSettings.Width, _HeightMapGenerator.NoiseSettings.Height);
            
            int width = _HeightMapGenerator.NoiseSettings.Width;
            int height = _HeightMapGenerator.NoiseSettings.Height;

            float halfWidth  = width * .5f;
            float halfHeight = height * .5f;

            int vertIdx = 0;
            Helpers.IteratePointsOnMap(width, height, (x, y) => {
                meshData.Vertices[vertIdx] = new Vector3(x - halfWidth, heightMap[x, y] * _HeightMultiplier, y - halfHeight);
                meshData.UVs[vertIdx] = new Vector2(x / (float) width, y / (float) height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertIdx, vertIdx + width + 1, vertIdx + width);
                    meshData.AddTriangle(vertIdx + width + 1, vertIdx, vertIdx + 1);
                }
                
                vertIdx++;
            });
            
            MeshFilter.sharedMesh = meshData.Get();
            MeshFilter.transform.localScale = new Vector3(width, 1, height);

            return meshData;
        }
    }
}