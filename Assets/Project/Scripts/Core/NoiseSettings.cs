using System;
using UnityEngine;

namespace NoiseGenerator.Core
{
    [Serializable]
    public class NoiseSettings
    {
        [Min(1)] public int Size = 200;

        public Vector2 Offset;

        [Min(.75f)] public float Scale = 30f;

        public int octaveAmount => Octaves.OctaveAmount;

        public bool OverrideOctaves = true;

        public OctaveList Octaves = new (4);

        [Range(.05f, 1f)] public float Persistence = .5f;
        public float Lacunarity = 2;

        public AnimationCurve HeightCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Warp Settings")]
        public bool WarpNoise;

        [Range(0f, 1f)] public float BlendValue = 1;
        public float f = 3.5f;

        public NoiseSettings(int size)
        {
            Size = size;

            Octaves = new OctaveList();
        }
    }
}
