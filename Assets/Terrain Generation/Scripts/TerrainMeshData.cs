using UnityEngine;

namespace NoiseGenerator.TerrainGeneration
{
    public class TerrainMeshData
    {
        public Vector3[] Vertices;
        public int[] Triangles;
        public Vector2[] UVs;

        private int _TriangleIndex;

        public TerrainMeshData(int size)
        {
            Vertices = new Vector3[size * size];
            Triangles = new int[(size - 1) * (size - 1) * 6];
            UVs = new Vector2[size * size];
        }
        
        public void AddTriangle(int a, int b, int c)
        {
            Triangles[_TriangleIndex] = a;
            Triangles[_TriangleIndex + 1] = b;
            Triangles[_TriangleIndex + 2] = c;

            _TriangleIndex += 3;
        }

        public Mesh Get()
        {
            return new Mesh
            {
                vertices = Vertices,
                triangles = Triangles,
                uv = UVs
            };
        }
    }
}