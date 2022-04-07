using System;
using UnityEngine;

namespace NoiseGenerator
{
    public class NoiseMapGenerator : MonoBehaviour
    {
        public NoiseSettings NoiseSettings;
        public bool AutoUpdate;

        private float[,] Generate(NoiseSettings noiseSettings)
        {
            float[,] noiseValues = new float[noiseSettings.Width, noiseSettings.Height];

            float max = float.MinValue;
            float min = float.MaxValue;

            for (int x = 0; x < noiseSettings.Width; x++)
            {
                for (int y = 0; y < noiseSettings.Height; y++)
                {
                    float amplitude = 1;
                    float freq = 1;
                    float noiseHeight = 0;

                    for (int o = 0; o < noiseSettings.OctaveAmount; o++)
                    {
                        Vector2 sample = new Vector2(
                            x / noiseSettings.Scale * freq + noiseSettings.Offset.x,
                            y / noiseSettings.Scale * freq + noiseSettings.Offset.y
                        );

                        float value = noiseSettings.WarpNoise && noiseSettings.WarpAmount != 0
                            ? Mathf.Lerp(
                                Noise.Evaluate(new Vector2(sample.x, sample.y)),
                                Noise.Warp(new Vector2(sample.x, sample.y), noiseSettings.f),
                                noiseSettings.WarpAmount) * 2 - 1
                            : Noise.Evaluate(new Vector2(sample.x, sample.y)) * 2 - 1;
                        noiseHeight += value * amplitude;

                        amplitude *= noiseSettings.Persistence;
                        freq *= noiseSettings.Lacunarity;
                    }

                    max = noiseHeight > max ? noiseHeight : max;
                    min = noiseHeight < min ? noiseHeight : min;

                    noiseValues[x, y] = noiseHeight;

                    noiseValues[x, y] = Mathf.InverseLerp(min, max, noiseValues[x, y]);
                }
            }

            return noiseValues;
        }

        public static Action OnGenerate;

        private void OnGen() => GetComponent<NoiseDisplay>().UpdateTex(Generate(NoiseSettings), NoiseSettings);

        private void OnValidate()
        {
            if (OnGenerate is null)
                OnGenerate += OnGen;

            if (AutoUpdate)
                OnGenerate?.Invoke();
        }
    }

    [Serializable]
    public struct NoiseSettings
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
        [Range(0, 1)] public float WarpAmount;
        public float f;
    }
}