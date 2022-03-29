using UnityEngine;

namespace NoiseGenerator
{
    public static class Noise
    {
        public static float Evaluate(Vector2 p) => Mathf.PerlinNoise(p.x, p.y);

        public static float Warp(Vector2 p, float f)
        {
            Vector2 q = new Vector2(Evaluate(p), Evaluate(p + new Vector2(5.2f, 1.3f)));

            return Evaluate(p + f * q);
        }
    }
}