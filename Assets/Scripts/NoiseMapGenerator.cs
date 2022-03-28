using UnityEngine;

public class NoiseMapGenerator : MonoBehaviour
{
    [System.Serializable]
    public record NoiseSettings
    {
        [Min(1)] public int Width;
        [Min(1)] public int Height;
        public Vector2 Offset;
        [Min(.75f)] public float Scale;
        [Range(1, 8)] public int OctaveAmount;
        [Range(.05f, 1f)] public float Persistence;
        public float Lacunarity;

        public bool WarpNoise = false;
        //[ConditionalHide("WarpNoise")] not working :(
        public WarpSettings _warpSettings;

        [System.Serializable]
        public struct WarpSettings
        {
            [Range(0, 1)] public float WarpAmount;
        }
    }

    [SerializeField] private NoiseSettings _noiseSettings;
    public bool AutoUpdate;


    private float[,] Generate(NoiseSettings noiseSettings)
    {
        float[,] noiseValues = new float[noiseSettings.Width, noiseSettings.Height];

        float max = float.MinValue;
        float min = float.MaxValue;

        for (var x = 0; x < noiseSettings.Width; x++)
        {
            for (var y = 0; y < noiseSettings.Height; y++)
            {
                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;

                for (int o = 0; o < noiseSettings.OctaveAmount; o++)
                {
                    float sampleX = x / noiseSettings.Scale * freq;
                    float sampleY = y / noiseSettings.Scale * freq;

                    float processedSampleX = sampleX + noiseSettings.Offset.x;
                    float processedSampleY = sampleY + noiseSettings.Offset.y;

                    float value = noiseSettings.WarpNoise
                        ? Mathf.Lerp(
                            Noise.Evaluate(new Vector2(processedSampleX, processedSampleY)),
                            Noise.Warp(new Vector2(processedSampleX, processedSampleY)),
                            noiseSettings._warpSettings.WarpAmount) * 2 - 1
                        : Noise.Evaluate(new Vector2(processedSampleX, processedSampleY)) * 2 - 1;
                    noiseHeight += value * amplitude;

                    amplitude *= noiseSettings.Persistence;
                    freq *= noiseSettings.Lacunarity;
                }
                max = noiseHeight > max ? noiseHeight : max;
                min = noiseHeight < min ? noiseHeight : min;

                noiseValues[x, y] = noiseHeight;
            }
        }

        for (int x = 0; x < noiseSettings.Width; x++)
        {
            for (int y = 0; y < noiseSettings.Height; y++)
            {
                noiseValues[x, y] = Mathf.InverseLerp(min, max, noiseValues[x, y]);
            }
        }

        return noiseValues;
    }

    public delegate void OnGenerate();
    public static OnGenerate onGenerate;

    private void OnGen() => GetComponent<NoiseDisplay>().UpdateTex(Generate(_noiseSettings), _noiseSettings);

    private void OnValidate()
    {
        onGenerate ??= OnGen;

        if (AutoUpdate) onGenerate.Invoke();
    }
}