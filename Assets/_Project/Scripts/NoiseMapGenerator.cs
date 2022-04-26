using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator
{
    public class NoiseMapGenerator : MonoBehaviour
    {
        public Preset Preset;
        public NoiseSettings NoiseSettings;
        public bool AutoGenerate;
        public bool AutoSave;
        
        private NoiseDisplay _noiseDisplay;
        
        public enum OctaveGenerationState
        {
            Auto,
            Manual
        }

        [SerializeField] private OctaveGenerationState _OctaveGenerationState;

        private float[,] GenerateNoiseMap(NoiseSettings noiseSettings)
        {
            float[,] noiseValues = new float[noiseSettings.Width, noiseSettings.Height];

            MinMax minMax = new ();

            IteratePointsOnMap(noiseSettings.Width, noiseSettings.Height, point =>
            {
                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;

                int x = point.x;
                int y = point.y;

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
}