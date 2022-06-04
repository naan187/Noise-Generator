using UnityEngine;

namespace NoiseGenerator.Core
{
    public class HeightMapGenerator : MonoBehaviour
    {
        [SerializeField] private NoisemapPreset _Preset;
        public NoiseSettings NoiseSettings;
        public bool AutoGenerate;
        public bool AutoSave;

        public PostGenerateEvent postGenerate { get; } = new();


        private float[] GenerateHeightMap(NoiseSettings noiseSettings)
        {
            float[] noiseValues = new float[noiseSettings.Size * noiseSettings.Size];

            MinMax minMax = new();

            if (noiseSettings.octaveAmount != noiseSettings.Octaves.length)
                noiseSettings.Octaves.Resize(noiseSettings.Octaves.OctaveAmount);

            var prng = new System.Random(noiseSettings.Seed);

            var globalOffset = new Vector2(
                prng.Next(-10000, 10000) + noiseSettings.Offset.x,
                prng.Next(-10000, 10000) + noiseSettings.Offset.y
            );

            float halfSize = noiseSettings.Size / 2f;

            Helpers.IteratePointsOnMap(noiseSettings.Size, (x, y, i) =>
            {
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

                    Vector2 sample = new(
                        (x - halfSize) / noiseSettings.Scale * freq + globalOffset.x,
                        (y - halfSize) / noiseSettings.Scale * freq + globalOffset.y
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

                noiseValues[i] = noiseHeight;
            });

            return noiseValues;
        }

        /// <summary>
        ///     Generates a heightmap without invoking Listener Methods
        /// </summary>
        /// <param name="size">the side-length of the generated heightmap</param>
        public float[] GenerateHeightMap(int size = 0)
        {
            int prevSize = NoiseSettings.Size;
            
            if (size is not 0)
                NoiseSettings.Size = size;

            var heightmap = GenerateHeightMap(NoiseSettings);

            NoiseSettings.Size = prevSize;

            return heightmap;
        }

        /// <summary>
        ///     Generates a heightmap and invokes Listener Methods
        /// </summary>
        /// <param name="size">the side-length of the generated heightmap</param>
        public float[] Generate(int size = 0)
        {
            if (size is not 0)
                NoiseSettings.Size = size;

            var heightMap = GenerateHeightMap(NoiseSettings);

            postGenerate?.Invoke(heightMap);

            return heightMap;
        }

        public void Save() => _Preset.NoiseSettings = NoiseSettings;

        public void Undo()
        {
            NoiseSettings = _Preset.NoiseSettings;
            Generate();
        }
    }
}