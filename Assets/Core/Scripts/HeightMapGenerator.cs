using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace NoiseGenerator.Core
{
    public class HeightMapGenerator : MonoBehaviour
    {
        public HeightmapPreset Preset;
        public bool RandomizeSeed;
        
        public NoiseSettings NoiseSettings = new (4);
        public bool AutoGenerate;
        public bool AutoSave;
        public bool UseComputeShader;
        
        public ComputeShader HeightMapComputeShader;

        public PostGenerateEvent postGenerate { get; } = new();


        private float[] GenerateHeightMapCPU(ref NoiseSettings settings)
        {
            float[] heightMap = new float[settings.Size * settings.Size];
            MinMax minMax = new MinMax{Max = float.MinValue, Min = float.MaxValue};

            if (settings.OctaveAmount != settings.Octaves.Length)
                settings.Octaves.Resize(settings.Octaves.OctaveAmount);

            var prng = new System.Random(settings.Seed);

            var globalOffset = new Vector2(
                prng.Next(-10000, 10000) + (settings.Offset.x + transform.localPosition.x),
                prng.Next(-10000, 10000) - (settings.Offset.y + transform.localPosition.z)
            );
             
            float halfSize = settings.Size / 2f;

            for (int i = 0; i < heightMap.Length; i++)
            {
                int x = i % settings.Size;
                int y = i / settings.Size;
                
                float weight = 1;
                float scale = settings.Scale / 30f;
                float noiseValue = 0;

                for (int o = 0; o < settings.OctaveAmount; o++)
                {
                    Vector2 samplePoint = 
                        new Vector2(x - halfSize, y - halfSize) / settings.Size * scale + globalOffset;

                    float value = Noise.Evaluate(samplePoint);

                    noiseValue += value * weight;
                    
                    weight *= settings.Persistence;
                    scale *= settings.Lacunarity;
                }

                minMax.Update(noiseValue);

                noiseValue = Mathf.InverseLerp(minMax.Min, minMax.Max, noiseValue);
                
                noiseValue = settings.HeightCurve.Evaluate(noiseValue);

                heightMap[i] = noiseValue;
            }
            
            return heightMap;
        }
        
        private float[] GenerateHeightMapGPU(ref NoiseSettings settings)
        {
            float[] heightMap = new float[settings.Size * settings.Size];

            ComputeBuffer heightMapBuffer = new ComputeBuffer(heightMap.Length, sizeof(float));
            heightMapBuffer.SetData(heightMap);
            HeightMapComputeShader.SetBuffer(0, "heightMap", heightMapBuffer);

            var minMax = new []{1000f * settings.OctaveAmount, 0f};
            ComputeBuffer minMaxBuffer = new ComputeBuffer(minMax.Length, sizeof(float));
            minMaxBuffer.SetData(minMax);
            HeightMapComputeShader.SetBuffer(0, "minMax", minMaxBuffer);
            
            HeightMapComputeShader.SetInt("seed", settings.Seed);
            HeightMapComputeShader.SetInt("mapSize", settings.Size);
            HeightMapComputeShader.SetFloat("noiseScale", settings.Scale / 50f);
            HeightMapComputeShader.SetInt("numOctaves", settings.OctaveAmount);
            HeightMapComputeShader.SetFloat("persistence", settings.Persistence);
            HeightMapComputeShader.SetFloat("lacunarity", settings.Lacunarity);
            
            var prng = new System.Random(settings.Seed);

            var globalOffset = new Vector2 (
                prng.Next(-10000, 10000) - (settings.Offset.y + transform.position.z) / settings.Size,
                prng.Next(-10000, 10000) + (settings.Offset.x + transform.position.x) / settings.Size
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
                heightMap[i] = settings.HeightCurve.Evaluate(Mathf.InverseLerp(min, max, heightMap[i]));
            
            return heightMap;
        }

        /// <summary>
        ///     Generates a heightmap without invoking Listener Methods
        /// </summary>
        /// <param name="size">the side-length of the generated heightmap</param>
        /// <param name="useComputeShader">whether to run the generation of the heightmap on the GPU or CPU</param>
        public float[] GenerateHeightMap(int size = 0, bool? useComputeShader = null)
        {
            if (RandomizeSeed)
                NoiseSettings.Seed = Random.Range(-100000, 100000);


            int prevSize = NoiseSettings.Size;

            if (size is not 0)
                NoiseSettings.Size = size;

            useComputeShader ??= UseComputeShader;
            
            var heightMap = 
                useComputeShader.Value 
                    ? GenerateHeightMapGPU(ref NoiseSettings)
                    : GenerateHeightMapCPU(ref NoiseSettings);

            NoiseSettings.Size = prevSize;
 
            return heightMap;
        }

        /// <summary>
        ///     Generates a heightmap and invokes Listener Methods
        /// </summary>
        /// <param name="size">the side-length of the generated heightmap</param>
        /// <param name="useComputeShader">whether to run the generation of the heightmap on the GPU or CPU</param>
        public float[] Generate(int size = 0, bool? useComputeShader = null)
        {
            if (RandomizeSeed)
                NoiseSettings.Seed = Random.Range(-100000, 100000);
            
            if (size is not 0)
                NoiseSettings.Size = size;

            useComputeShader ??= UseComputeShader;

            var heightMap = 
                useComputeShader.Value 
                                      ? GenerateHeightMapGPU(ref NoiseSettings)
                                      : GenerateHeightMapCPU(ref NoiseSettings);
            
            postGenerate?.Invoke(heightMap);

            return heightMap;
        }
        
        /// <summary>
        ///     Generates a heightmap without invoking Listener Methods
        /// </summary>
        /// <param name="settings">the heightmap's settings</param>
        /// <param name="useComputeShader">whether to run the generation of the heightmap on the GPU or CPU</param>
        public float[] GenerateHeightMap(ref NoiseSettings settings, bool? useComputeShader = null)
        {
            if (RandomizeSeed)
                settings.Seed = Random.Range(-100000, 100000);

            useComputeShader ??= UseComputeShader;
            
            var heightMap = useComputeShader.Value ? GenerateHeightMapGPU(ref settings) : GenerateHeightMapCPU(ref settings);
 
            return heightMap;
        }

        /// <summary>
        ///     Generates a heightmap and invokes Listener Methods
        /// </summary>
        /// <param name="settings">the heightmap's settings</param>
        /// <param name="useComputeShader">whether to run the generation of the heightmap on the GPU or CPU</param>
        public float[] Generate(ref NoiseSettings settings, bool? useComputeShader = null)
        {
            if (RandomizeSeed)
                settings.Seed = Random.Range(-100000, 100000);

            useComputeShader ??= UseComputeShader;

            var heightMap = useComputeShader.Value ? GenerateHeightMapGPU(ref settings) : GenerateHeightMapCPU(ref settings);
            
            postGenerate?.Invoke(heightMap);

            return heightMap;
        }

        public void Save() => Preset.NoiseSettings = new(NoiseSettings);

        public void Undo()
        {
            NoiseSettings = new(Preset.NoiseSettings);
            Generate();
        }
    }
}