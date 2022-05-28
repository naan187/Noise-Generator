using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator.Core
{
    public class HeightMapGenerator : MonoBehaviour
    {
        [SerializeField]
        private NoisemapPreset _Preset;
        public NoiseSettings NoiseSettings;
        public bool AutoGenerate;
        public bool AutoSave;

        [SerializeField]
        private bool _ScaleGameObject;
        [SerializeField]
        private float _ScaleFactor;

        public PostGenerateEvent postGenerate { get; } = new ();

        
        private float[,] GenerateHeightMap(NoiseSettings noiseSettings)
        {
            float[,] noiseValues = new float[noiseSettings.Size, noiseSettings.Size];

            MinMax minMax = new ();

            if (noiseSettings.octaveAmount != noiseSettings.Octaves.length)
                noiseSettings.Octaves.Resize(noiseSettings.Octaves.OctaveAmount);

            var prng = new System.Random(noiseSettings.Seed);
            
            var globalOffset = new Vector2(
                prng.Next(-10000, 10000) + noiseSettings.Offset.x,
                prng.Next(-10000, 10000) + noiseSettings.Offset.y
            );
            Helpers.IteratePointsOnMap(noiseSettings.Size, (x, y) => {
                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;

                foreach (Octave octave in noiseSettings.Octaves)
                {
                    amplitude = noiseSettings.OverrideOctaves ? amplitude : octave.Amplitude;
                    freq = noiseSettings.OverrideOctaves ? freq : octave.Frequency;

                    if (noiseSettings.OverrideOctaves)
                    {
                        amplitude *= noiseSettings.Persistence;
                        freq *= noiseSettings.Lacunarity;
                        
                        octave.Amplitude = amplitude;
                        octave.Frequency = freq;
                    }

                    Vector2 sample = new (
                        x / noiseSettings.Scale * freq + globalOffset.x,
                        y / noiseSettings.Scale * freq + globalOffset.y
                    );

                    float value = noiseSettings.WarpNoise && noiseSettings.BlendValue != 0
                        ? Mathf.Lerp(
                            Noise.Evaluate(new Vector2(sample.x, sample.y)),
                            Noise.Warp(new Vector2(sample.x, sample.y), noiseSettings.f),
                            noiseSettings.BlendValue) * 2 - 1
                        : Noise.Evaluate(new Vector2(sample.x, sample.y)) * 2 - 1;

                    noiseHeight += value * amplitude;
                }

                minMax.Update(noiseHeight);

                noiseHeight = Mathf.InverseLerp(minMax.Min, minMax.Max, noiseHeight);

                noiseHeight = noiseSettings.HeightCurve.Evaluate(noiseHeight);
                
                noiseValues[x, y] = noiseHeight;
            });

            if (_ScaleGameObject)
                transform.localScale = new Vector3(noiseSettings.Size * _ScaleFactor, 1, noiseSettings.Size * _ScaleFactor);

            return noiseValues;
        }

        public float[,] Generate(int size = 0)
        {
            if (size is not 0)
            {
                NoiseSettings.Size = size;
            }

            var heightMap = GenerateHeightMap(NoiseSettings);

            postGenerate?.Invoke(heightMap);
            
            return heightMap;
        }

        public void Save() => _Preset.NoiseSettings = NoiseSettings;
        public void Undo() {
            NoiseSettings = _Preset.NoiseSettings;
            Generate();
        }
    }
}