using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator
{

    [Serializable]
    public class NoiseSettings
    {
        [Min(1)] public int Width;
        [Min(1)] public int Height;

        public Vector2 Offset;

        [Min(.75f)] public float Scale;

        public int OctaveAmount
        {
            get => Octaves.OctaveAmount;
            set => Octaves.OctaveAmount = value;
        }

        [SerializeField] private bool _OverrideOctaves;

        public OctaveArray Octaves;

        [Range(.05f, 1f)] public float Persistence;
        public float Lacunarity;

        public AnimationCurve HeightCurve;

        [Header("Warp Settings")]
        public bool WarpNoise;
        public bool Blend;

        [Range(0f, 1f)] public float BlendAmount;
        public float f;

        public NoiseSettings(int width, int height)
        {
            Width = width;
            Height = height;

            Octaves = new OctaveArray();
        }
    }
}
