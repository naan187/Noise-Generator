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
        private MeshGenerator _MeshGenerator;


        private float[,] GenerateHeightMap(NoiseSettings noiseSettings)
        {
            float[,] noiseValues = new float[noiseSettings.Width, noiseSettings.Height];

            MinMax minMax = new ();

            if (noiseSettings.octaveAmount != noiseSettings.Octaves.length)
                noiseSettings.Octaves.Resize(noiseSettings.Octaves.OctaveAmount);

            Helpers.IteratePointsOnMap(noiseSettings.Width, noiseSettings.Height, (x, y) => {
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

            transform.localScale = new Vector3(noiseSettings.Width * .5f, 1, noiseSettings.Height * .5f);

            var meshGenerator = GetComponent<MeshGenerator>();
            if (meshGenerator)
                meshGenerator.GenerateMesh(noiseValues);
            
            return noiseValues;
        }

        public float[,] Generate(Vector2? mapDimensions = null)
        {
            if (mapDimensions is not null)
            {
                NoiseSettings.Width = (int) mapDimensions.Value.x;
                NoiseSettings.Height = (int) mapDimensions.Value.y;
            }
            
            var heightMap = GenerateHeightMap(NoiseSettings);

            _NoiseDisplay?.UpdateTex(heightMap);

            return heightMap;
        }

        public void Save() => _Preset.NoiseSettings = NoiseSettings;
        public void Undo() {
            NoiseSettings = _Preset.NoiseSettings;
            Generate();
        }

        private void OnValidate()
        {
            _NoiseDisplay ??= GetComponent<NoiseDisplay>();
        }
    }
}