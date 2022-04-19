using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator
{
    public class NoiseMapGenerator : MonoBehaviour
    {
        public Preset Preset;
        [Header("Noise Settings")]
        public NoiseSettings NoiseSettings;
        public bool AutoGenerate;
        public bool AutoSave;
        
        private NoiseDisplay _noiseDisplay;

        private float[,] GenerateNoiseMap(NoiseSettings noiseSettings)
        {
            float[,] noiseValues = new float[noiseSettings.MapDimensions.x, noiseSettings.MapDimensions.y];

            float max = float.MinValue;
            float min = float.MaxValue;

            for (int x = 0; x < noiseSettings.MapDimensions.x; x++)
            {
                for (int y = 0; y < noiseSettings.MapDimensions.y; y++)
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

                        float value = noiseSettings.WarpNoise && noiseSettings.BlendAmount != 0
                            ? Mathf.Lerp(
                                Noise.Evaluate(new Vector2(sample.x, sample.y)),
                                Noise.Warp(new Vector2(sample.x, sample.y), noiseSettings.f),
                                noiseSettings.BlendAmount) * 2 - 1
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
        
        public void Generate() => _noiseDisplay.UpdateTex(GenerateNoiseMap(NoiseSettings), NoiseSettings);
        public void Save() => Preset.NoiseSettings = NoiseSettings;
        public void Undo() {
            NoiseSettings = Preset.NoiseSettings;
            Generate();
        }

        private void OnValidate()
        {
            _noiseDisplay ??= GetComponent<NoiseDisplay>();

            if (AutoGenerate)
                Generate();
            if (AutoSave)
                Save();
        }
    }

    [Serializable]
    public struct NoiseSettings
    {
        [Min(1), SerializeField] private int _width;
        [Min(1), SerializeField] private int _height;

        public Vector2Int MapDimensions
        {
            get => new (_width, _height);
            set {
                _width = value.x;
                _height = value.y;
            }
        }

        public Vector2 Offset;

        [Min(.75f)] public float Scale;
        [Range(1, 8)] public int OctaveAmount;
        [Range(.05f, 1f)] public float Persistence;
        public float Lacunarity;

        [Header("Warp Settings")]
        public bool WarpNoise;
        public bool Blend;

        [FormerlySerializedAs("WarpAmount")] [Range(0f, 1f)] public float BlendAmount;
        public float f;
    }
}