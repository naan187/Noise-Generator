using UnityEngine;

namespace NoiseGenerator.Core
{
    public static class Noise
    {
        public static float Evaluate(Vector2 p) => Mathf.PerlinNoise(p.x, p.y);
    }
}