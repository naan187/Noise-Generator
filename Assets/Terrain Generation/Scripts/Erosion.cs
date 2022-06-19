//Original Version by Sebastian Lague at: https://github.com/SebLague/Hydraulic-Erosion
//Modified by Nathan's Codes
/*
 * MIT License
 * 
 * Copyright (c) 2019 Sebastian Lague
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 */

using System.Collections.Generic;
using UnityEngine;

using NoiseGenerator.Core;
using Random = UnityEngine.Random;

namespace NoiseGenerator.TerrainGeneration
{
    public class Erosion : MonoBehaviour
    {
        public bool printTimers;

        public HeightMapGenerator heightMapGenerator;
        public TerrainGenerator terrainGenerator;
        private NoiseSettings noiseSettings => heightMapGenerator.NoiseSettings;
        public int mapSize
        {
            get => noiseSettings.Size;
            private set => noiseSettings.Size = value;
        }

        [Header("Erosion Settings")]
        public ComputeShader ErosionComputeShader;
        public int NumErosionIterations = 50000;
        public int ErosionBrushRadius = 3;

        public int MaxLifetime = 30;
        public float SedimentCapacityFactor = 3;
        public float MinSedimentCapacity = .01f;
        public float DepositSpeed = 0.3f;
        public float ErodeSpeed = 0.3f;

        public float EvaporateSpeed = .01f;
        public float Gravity = 4;
        public float StartSpeed = 1;
        public float StartWater = 1;
        [Range(0, 1)]
        public float Inertia = 0.3f;

        [SerializeField]
        private bool _Erode;

        // Internal
        private float[] _Map;
        private Mesh _Mesh;
        private int _BorderSize;

        private MeshRenderer _MeshRenderer;
        private MeshFilter _MeshFilter;

        public void GenerateHeightMap() 
        {
            _BorderSize = ErosionBrushRadius * 2;
            _Map = heightMapGenerator.GenerateHeightMap(mapSize);
        }
        
        public float[] Erode(float[] heightmap = null)
        {
            switch (heightmap)
            {
                case null:
                    GenerateHeightMap();
                    break;
                default:
                    _Map = heightmap;
                    break;
            }
            
            if (!_Erode)
                return _Map;

            int numThreads = NumErosionIterations / 1024;

            // Create brush
            List<int> brushIndexOffsets = new List<int>();
            List<float> brushWeights = new List<float>();

            float weightSum = 0;
            for (int brushY = -ErosionBrushRadius; brushY <= ErosionBrushRadius; brushY++)
            {
                for (int brushX = -ErosionBrushRadius; brushX <= ErosionBrushRadius; brushX++)
                {
                    float sqrDst = brushX * brushX + brushY * brushY;
                    if (!(sqrDst < ErosionBrushRadius * ErosionBrushRadius))
                        continue;
                    
                    brushIndexOffsets.Add(brushY * mapSize + brushX);
                    float brushWeight = 1 - Mathf.Sqrt(sqrDst) / ErosionBrushRadius;
                    weightSum += brushWeight;
                    brushWeights.Add(brushWeight);
                }
            }

            for (int i = 0; i < brushWeights.Count; i++)
                brushWeights[i] /= weightSum;

            // Send brush data to compute shader
            ComputeBuffer brushIndexBuffer = new ComputeBuffer(brushIndexOffsets.Count, sizeof(int));
            ComputeBuffer brushWeightBuffer = new ComputeBuffer(brushWeights.Count, sizeof(int));
            brushIndexBuffer.SetData(brushIndexOffsets);
            brushWeightBuffer.SetData(brushWeights);
            ErosionComputeShader.SetBuffer(0, "brushIndices", brushIndexBuffer);
            ErosionComputeShader.SetBuffer(0, "brushWeights", brushWeightBuffer);

            // Generate random indices for droplet placement
            int[] randomIndices = new int[NumErosionIterations];
            for (int i = 0; i < NumErosionIterations; i++)
            {
                int randomX = Random.Range(ErosionBrushRadius, mapSize + ErosionBrushRadius);
                int randomY = Random.Range(ErosionBrushRadius, mapSize + ErosionBrushRadius);
                randomIndices[i] = randomY * mapSize + randomX;
            }

            // Send random indices to compute shader
            ComputeBuffer randomIndexBuffer = new ComputeBuffer(randomIndices.Length, sizeof(int));
            randomIndexBuffer.SetData(randomIndices);
            ErosionComputeShader.SetBuffer(0, "randomIndices", randomIndexBuffer);
            
            // Heightmap buffer
            ComputeBuffer mapBuffer = new ComputeBuffer(_Map.Length, sizeof(float));
            mapBuffer.SetData(_Map);
            ErosionComputeShader.SetBuffer(0, "map", mapBuffer);
            
            // Settings
            ErosionComputeShader.SetInt("borderSize", _BorderSize);
            ErosionComputeShader.SetInt("mapSize", mapSize);
            ErosionComputeShader.SetInt("brushLength", brushIndexOffsets.Count);
            ErosionComputeShader.SetInt("maxLifetime", MaxLifetime);
            ErosionComputeShader.SetFloat("inertia", Inertia);
            ErosionComputeShader.SetFloat("sedimentCapacityFactor", SedimentCapacityFactor);
            ErosionComputeShader.SetFloat("minSedimentCapacity", MinSedimentCapacity);
            ErosionComputeShader.SetFloat("depositSpeed", DepositSpeed);
            ErosionComputeShader.SetFloat("erodeSpeed", ErodeSpeed);
            ErosionComputeShader.SetFloat("evaporateSpeed", EvaporateSpeed);
            ErosionComputeShader.SetFloat("gravity", Gravity);
            ErosionComputeShader.SetFloat("startSpeed", StartSpeed);
            ErosionComputeShader.SetFloat("startWater", StartWater);

            // Run compute shader
            ErosionComputeShader.Dispatch(0, numThreads, 1, 1);
            mapBuffer.GetData(_Map);

            // Release buffers
            mapBuffer.Release();
            randomIndexBuffer.Release();
            brushIndexBuffer.Release();
            brushWeightBuffer.Release();

            return _Map;
        }
        
        private void ErodeInternal(float[] heightmap)
        {
            if (!_Erode)
                return;
            
            _Map = heightmap;
            
            int numThreads = NumErosionIterations / 1024;

            // Create brush
            List<int> brushIndexOffsets = new List<int>();
            List<float> brushWeights = new List<float>();

            float weightSum = 0;
            for (int brushY = -ErosionBrushRadius; brushY <= ErosionBrushRadius; brushY++)
            {
                for (int brushX = -ErosionBrushRadius; brushX <= ErosionBrushRadius; brushX++)
                {
                    float sqrDst = brushX * brushX + brushY * brushY;
                    if (!(sqrDst < ErosionBrushRadius * ErosionBrushRadius))
                        continue;
                    
                    brushIndexOffsets.Add(brushY * mapSize + brushX);
                    float brushWeight = 1 - Mathf.Sqrt(sqrDst) / ErosionBrushRadius;
                    weightSum += brushWeight;
                    brushWeights.Add(brushWeight);
                }
            }

            for (int i = 0; i < brushWeights.Count; i++)
                brushWeights[i] /= weightSum;

            // Send brush data to compute shader
            ComputeBuffer brushIndexBuffer = new ComputeBuffer(brushIndexOffsets.Count, sizeof(int));
            ComputeBuffer brushWeightBuffer = new ComputeBuffer(brushWeights.Count, sizeof(int));
            brushIndexBuffer.SetData(brushIndexOffsets);
            brushWeightBuffer.SetData(brushWeights);
            ErosionComputeShader.SetBuffer(0, "brushIndices", brushIndexBuffer);
            ErosionComputeShader.SetBuffer(0, "brushWeights", brushWeightBuffer);

            // Generate random indices for droplet placement
            int[] randomIndices = new int[NumErosionIterations];
            for (int i = 0; i < NumErosionIterations; i++)
            {
                int randomX = Random.Range(ErosionBrushRadius, mapSize + ErosionBrushRadius);
                int randomY = Random.Range(ErosionBrushRadius, mapSize + ErosionBrushRadius);
                randomIndices[i] = randomY * mapSize + randomX;
            }

            // Send random indices to compute shader
            ComputeBuffer randomIndexBuffer = new ComputeBuffer(randomIndices.Length, sizeof(int));
            randomIndexBuffer.SetData(randomIndices);
            ErosionComputeShader.SetBuffer(0, "randomIndices", randomIndexBuffer);
            
            // Heightmap buffer
            ComputeBuffer mapBuffer = new ComputeBuffer(_Map.Length, sizeof(float));
            mapBuffer.SetData(_Map);
            ErosionComputeShader.SetBuffer(0, "map", mapBuffer);
            
            // Settings
            ErosionComputeShader.SetInt("borderSize", _BorderSize);
            ErosionComputeShader.SetInt("mapSize", mapSize);
            ErosionComputeShader.SetInt("brushLength", brushIndexOffsets.Count);
            ErosionComputeShader.SetInt("maxLifetime", MaxLifetime);
            ErosionComputeShader.SetFloat("inertia", Inertia);
            ErosionComputeShader.SetFloat("sedimentCapacityFactor", SedimentCapacityFactor);
            ErosionComputeShader.SetFloat("minSedimentCapacity", MinSedimentCapacity);
            ErosionComputeShader.SetFloat("depositSpeed", DepositSpeed);
            ErosionComputeShader.SetFloat("erodeSpeed", ErodeSpeed);
            ErosionComputeShader.SetFloat("evaporateSpeed", EvaporateSpeed);
            ErosionComputeShader.SetFloat("gravity", Gravity);
            ErosionComputeShader.SetFloat("startSpeed", StartSpeed);
            ErosionComputeShader.SetFloat("startWater", StartWater);

            // Run compute shader
            ErosionComputeShader.Dispatch(0, numThreads, 1, 1);
            mapBuffer.GetData(_Map);

            // Release buffers
            mapBuffer.Release();
            randomIndexBuffer.Release();
            brushIndexBuffer.Release();
            brushWeightBuffer.Release();
            
            ConstructMesh();
        }

        public void ConstructMesh() => terrainGenerator.GenerateMesh(_Map);

        private void OnValidate() => heightMapGenerator.postGenerate.Register(ErodeInternal, 4998);
    }
}