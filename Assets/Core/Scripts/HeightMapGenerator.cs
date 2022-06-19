using System;
using JetBrains.Annotations;
using UnityEngine;

namespace NoiseGenerator.Core
{
    public class HeightMapGenerator : MonoBehaviour
    {
        [SerializeField]
        private NoisemapPreset _Preset;
        [SerializeField]
        private bool _RandomizeSeed;
        
        public NoiseSettings NoiseSettings;
        public bool AutoGenerate;
        public bool AutoSave;
        public bool UseComputeShader;
        
        public ComputeShader HeightMapComputeShader;

        public PostGenerateEvent postGenerate { get; } = new();

        private float[] GenerateHeightMapCPU()
        {
            float[] heightMap = new float[NoiseSettings.Size * NoiseSettings.Size];
            MinMax minMax = new MinMax{Max = 1000f * 5f, Min = 0f};

            NoiseSettings.UpdateValues();
            
            if (NoiseSettings.OctaveAmount != NoiseSettings.Octaves.length)
                NoiseSettings.Octaves.Resize(NoiseSettings.Octaves.OctaveAmount);

            var prng = new System.Random(NoiseSettings.Seed);

            var globalOffset = new Vector2(
                prng.Next(-10000, 10000) + (NoiseSettings.Offset.x + transform.localPosition.x),
                prng.Next(-10000, 10000) - (NoiseSettings.Offset.y + transform.localPosition.z)
            );
             
            float halfSize = NoiseSettings.Size / 2f;

            for (int i = 0; i < heightMap.Length; i++)
            {
                int x = i % NoiseSettings.Size;
                int y = i / NoiseSettings.Size;
                
                float amplitude = 1;
                float freq = 1;
                float noiseValue = 0;

                foreach (Octave octave in NoiseSettings.Octaves)
                {
                    amplitude *= NoiseSettings.Persistence;
                    freq *= NoiseSettings.Lacunarity;

                    octave.Amplitude = amplitude;
                    octave.Frequency = freq;

                    Vector2 sample = (new Vector2(x - halfSize, y - halfSize) + globalOffset) / NoiseSettings.Scale * freq;

                    float value = NoiseSettings.WarpNoise && NoiseSettings.BlendValue != 0
                        ? Mathf.Lerp(
                            Noise.Evaluate(sample),
                            Noise.Warp(sample, NoiseSettings.f),
                            NoiseSettings.BlendValue) * 2 - 1
                        : Noise.Evaluate(sample) * 2 - 1;

                    noiseValue += value * amplitude;
                }

                minMax.Update(noiseValue);

                noiseValue = Mathf.InverseLerp(minMax.Min, minMax.Max, noiseValue);

                noiseValue = NoiseSettings.HeightCurve.Evaluate(noiseValue);

                heightMap[i] = noiseValue;
            }
            
            return heightMap;
        }
        
        private float[] GenerateHeightMapGPU()
        {
            float[] heightMap = new float[NoiseSettings.Size * NoiseSettings.Size];

            ComputeBuffer heightMapBuffer = new ComputeBuffer(heightMap.Length, sizeof(float));
            heightMapBuffer.SetData(heightMap);
            HeightMapComputeShader.SetBuffer(0, "heightMap", heightMapBuffer);

            var minMax = new []{1000f * NoiseSettings.OctaveAmount, 0f};
            ComputeBuffer minMaxBuffer = new ComputeBuffer(minMax.Length, sizeof(float));
            minMaxBuffer.SetData(minMax);
            HeightMapComputeShader.SetBuffer(0, "minMax", minMaxBuffer);
            
            HeightMapComputeShader.SetInt("seed", NoiseSettings.Seed);
            HeightMapComputeShader.SetInt("mapSize", NoiseSettings.Size);
            HeightMapComputeShader.SetFloat("noiseScale", NoiseSettings.Scale / 50);
            HeightMapComputeShader.SetInt("numOctaves", NoiseSettings.OctaveAmount);
            HeightMapComputeShader.SetFloat("persistence", NoiseSettings.Persistence);
            HeightMapComputeShader.SetFloat("lacunarity", NoiseSettings.Lacunarity);
            
            var prng = new System.Random(NoiseSettings.Seed);

            var globalOffset = new Vector2 (
                prng.Next(-10000, 10000) + (NoiseSettings.Offset.x + transform.position.x),
                prng.Next(-10000, 10000) - (NoiseSettings.Offset.y + transform.position.z)
            );
            
            HeightMapComputeShader.SetVector("globalOffset", globalOffset);

            HeightMapComputeShader.Dispatch(0, heightMap.Length-1, 1, 1);
            
            heightMapBuffer.GetData(heightMap);
            minMaxBuffer.GetData(minMax);
            heightMapBuffer.Release();
            minMaxBuffer.Release();

            //normalize
            float min = minMax[0];
            float max = minMax[1];
            
            for (int i = 0; i < heightMap.Length; i++)
                heightMap[i] = NoiseSettings.HeightCurve.Evaluate(Mathf.InverseLerp(min, max, heightMap[i]));
            
            return heightMap;
        }

        /// <summary>
        ///     Generates a heightmap without invoking Listener Methods
        /// </summary>
        /// <param name="size">the side-length of the generated heightmap</param>
        public float[] GenerateHeightMap(int size = 0)
        {
            if (_RandomizeSeed)
                NoiseSettings.Seed = UnityEngine.Random.Range(-100000, 100000);
            
            int prevSize = NoiseSettings.Size;
            
            if (size is not 0)
                NoiseSettings.Size = size;

            var heightmap = UseComputeShader ? GenerateHeightMapGPU() : GenerateHeightMapCPU();

            NoiseSettings.Size = prevSize;
 
            return heightmap;
        }

        /// <summary>
        ///     Generates a heightmap and invokes Listener Methods
        /// </summary>
        /// <param name="size">the side-length of the generated heightmap</param>
        public float[] Generate(int size = 0)
        {
            if (_RandomizeSeed)
                NoiseSettings.Seed = UnityEngine.Random.Range(-100000, 100000);
            
            if (size is not 0)
                NoiseSettings.Size = size;

            var heightMap = UseComputeShader ? GenerateHeightMapGPU() : GenerateHeightMapCPU();
            
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