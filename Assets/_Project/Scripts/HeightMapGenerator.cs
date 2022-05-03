using System;
using UnityEngine;

namespace NoiseGenerator
{
    public class HeightMapGenerator : MonoBehaviour
    {
        [SerializeField]
        private Preset _Preset;
        public NoiseSettings NoiseSettings;
        public bool AutoGenerate;
        public bool AutoSave;
        
        private NoiseDisplay _NoiseDisplay;
        
        private float[,] GenerateHeightMap(NoiseSettings noiseSettings)
        {
            float[,] noiseValues = new float[noiseSettings.Width, noiseSettings.Height];

            MinMax minMax = new ();

            if (noiseSettings.octaveAmount != noiseSettings.Octaves.length)
                noiseSettings.Octaves.Resize(noiseSettings.Octaves.OctaveAmount);

            IteratePointsOnMap(noiseSettings.Width, noiseSettings.Height, point =>
            {
                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;

                int x = point.x;
                int y = point.y;

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

        private void IteratePointsOnMap(int width, int height, Action<Vector2Int> action)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    action(new Vector2Int(x, y));
                }
            }
        }
        
        
        public void Generate() => _NoiseDisplay.UpdateTex(GenerateHeightMap(NoiseSettings), NoiseSettings);
        public void Save() => _Preset.NoiseSettings = NoiseSettings;
        public void Undo() {
            NoiseSettings = _Preset.NoiseSettings;
            Generate();
        }

        private void OnValidate()
        {
            _NoiseDisplay ??= GetComponent<NoiseDisplay>();

            if (AutoGenerate)
                Generate();
            if (AutoSave)
                Save();
        }
    }
}