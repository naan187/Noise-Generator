using System;
using UnityEngine;

namespace NoiseGenerator.Core
{
    [Serializable]
    public class NoiseSettings
    {
        public int Seed;

        [Range(16, 256)]
        public int Size;

        public Vector2 Offset;

        [Min(.75f)]
        public float Scale;

        [Range(1, 8)]
        public int OctaveAmount = 4;

        public OctaveList Octaves;

        [Range(0f, 1f)]
        public float Persistence;
        public float Lacunarity;
        
        public AnimationCurve HeightCurve;

        public NoiseSettings() { }

        public NoiseSettings(NoiseSettings source)
        {
            Seed = source.Seed;
            Size = source.Size;
            Offset = source.Offset;
            Scale = source.Scale;
            Octaves = source.Octaves;
            Octaves.OctaveAmount = source.Octaves.OctaveAmount;
            Persistence = source.Persistence;
            Lacunarity = source.Lacunarity;
            HeightCurve = source.HeightCurve;
        }
    }
}
