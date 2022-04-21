using System;
using UnityEngine;

namespace NoiseGenerator
{
    [Serializable]
    public class NoiseSettings
    {
        [Min(1)] public int Width;
        [Min(1)] public int Height;

        public Vector2 Offset;

        [Min(.75f)] public float Scale;
        [Range(1, 8)] public int OctaveAmount;
        [Range(.05f, 1f)] public float Persistence;
        public float Lacunarity;

        [Header("Warp Settings")]
        public bool WarpNoise;
        public bool Blend;

        [Range(0f, 1f)] public float BlendAmount;
        public float f;

        public NoiseSettings(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
