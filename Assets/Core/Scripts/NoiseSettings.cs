using System;
using UnityEngine;

namespace NoiseGenerator.Core
{
    [Serializable]
    public class NoiseSettings
    {
        public int Seed;

        //[Range(1, 256)]
        public int Size = 200;

        public Vector2 Offset;

        [Min(.75f)]
        public float Scale = 30f;

        [Range(1, 8)]
        public int OctaveAmount = 4;

        public OctaveList Octaves = new (4);

        [Range(.05f, 1f)]
        public float Persistence = .5f;
        public float Lacunarity = 2;

        public AnimationCurve HeightCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Warp Settings")]
        public bool WarpNoise;

        [Range(0f, 1f)] public float BlendValue = 1;
        public float f = 3.5f;

        public void UpdateValues() => Octaves.OctaveAmount = OctaveAmount;
    }
}
