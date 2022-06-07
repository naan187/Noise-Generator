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
                uv = UVs,
                normals = CalculateNormals()
            };
            return mesh;
        }
        
        public Vector3[] CalculateNormals()
        {
            Vector3[] vertexNormals = new Vector3[Vertices.Length];

            for (int i = 0; i < vertexNormals.Length; i++)
            {
                int normalTriangleIndex = i * 3;

                int vertexIndexA = Triangles[normalTriangleIndex];
                int vertexIndexB = Triangles[normalTriangleIndex + 1];
                int vertexIndexC = Triangles[normalTriangleIndex + 2];

                Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

                (vertexNormals[vertexIndexA] += triangleNormal).Normalize();
                (vertexNormals[vertexIndexB] += triangleNormal).Normalize();
                (vertexNormals[vertexIndexC] += triangleNormal).Normalize();
            }

            return vertexNormals;
        }

        private Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
        {
            Vector3 pointA = Vertices[indexA];
            Vector3 pointB = Vertices[indexB];
            Vector3 pointC = Vertices[indexC];

            Vector3 sideAB = pointB - pointA;
            Vector3 sideAC = pointC - pointA;

            return Vector3.Cross(sideAB, sideAC).normalized;
        }
    }
}