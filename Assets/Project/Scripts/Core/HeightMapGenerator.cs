using UnityEngine;

namespace NoiseGenerator.Core
{
    public static class HeightMapGenerator
    {
        public static float[,] GenerateHeightMap(NoiseSettings noiseSettings)
        {
            float[,] noiseValues = new float[noiseSettings.Size, noiseSettings.Size];

            MinMax minMax = new ();

            if (noiseSettings.octaveAmount != noiseSettings.Octaves.length)
                noiseSettings.Octaves.Resize(noiseSettings.Octaves.OctaveAmount);

            Helpers.IteratePointsOnMap(noiseSettings.Size, (x, y) => {
                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;

                foreach (Octave o in noiseSettings.Octaves)
                {
                    amplitude = noiseSettings.OverrideOctaves ? amplitude : o.Amplitude;
                    freq = noiseSettings.OverrideOctaves ? freq : o.Frequency;

                    if (noiseSettings.OverrideOctaves)
                    {
                        amplitude *= noiseSettings.Persistence;
                        freq *= noiseSettings.Lacunarity;
                        
                        o.Amplitude = amplitude;
                        o.Frequency = freq;
                    }

                    Vector2 sample = new (
                        x / noiseSettings.Scale * freq + noiseSettings.Offset.x,
                        y / noiseSettings.Scale * freq + noiseSettings.Offset.y
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

            return noiseValues;
        }
        
        public static float[,] GenerateHeightMap(int size, int octaveAmount)
        {
            float[,] noiseValues = new float[size, size];

            MinMax minMax = new ();

            var octaves = new OctaveList(octaveAmount);

            Helpers.IteratePointsOnMap(size, (x, y) => {
                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;

                foreach (Octave o in octaves)
                {
                    o.Amplitude = amplitude *= .5f;
                    o.Frequency = freq *= 2f;

                    Vector2 sample = new (
                        x * freq,
                        y * freq
                    );

                    float value = Noise.Evaluate(new Vector2(sample.x, sample.y)) * 2 - 1;

                    noiseHeight += value * amplitude;
                }

                minMax.Update(noiseHeight);

                noiseHeight = Mathf.InverseLerp(minMax.Min, minMax.Max, noiseHeight);

                noiseValues[x, y] = noiseHeight;
            });

            return noiseValues;
        }
        
    }
}