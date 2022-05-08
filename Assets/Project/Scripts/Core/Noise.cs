using UnityEngine;

namespace NoiseGenerator.Core
{
    public static class Noise
    {
        public static float Evaluate(Vector2 p) => Mathf.PerlinNoise(p.x, p.y);

        public static float Warp(Vector2 p, float f)
        {
            Vector2 q = new Vector2(Evaluate(p), Evaluate(p + new Vector2(5.2f, 1.3f)));

            return Evaluate(p + f * q);
        }

        public static float[,] EvaluateFromComputeShader(int size, ComputeShader noiseShader)
        {
            float[,] result = new float[size, size];

            

            return result;
        }
    }
}