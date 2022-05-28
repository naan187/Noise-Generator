using UnityEngine;

namespace NoiseGenerator.TerrainGeneration
{
    public class TerrainMeshData
    {
        public Vector3[] Vertices;
        public int[] Triangles;
        public Vector2[] UVs;

        private int _TriangleIndex;

        public TerrainMeshData(int width, int height)
        {
            Vertices = new Vector3[width * height];
            Triangles = new int[(width - 1) * (height - 1) * 6];
            UVs = new Vector2[width * height];
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
            var mesh = new Mesh
            {
                vertices = Vertices,
                triangles = Triangles,
                uv = UVs
            };
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}