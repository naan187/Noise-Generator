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

using System;
using System.Collections.Generic;
using UnityEngine;

using NoiseGenerator.Core;
using Random = UnityEngine.Random;

namespace NoiseGenerator.TerrainGeneration
{
    public class Eroder : MonoBehaviour
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

        public Material material;
        
        [Header("Erosion Settings")]
        public ComputeShader erosion;
        public int numErosionIterations = 50000;
        public int erosionBrushRadius = 3;

        public int maxLifetime = 30;
        public float sedimentCapacityFactor = 3;
        public float minSedimentCapacity = .01f;
        public float depositSpeed = 0.3f;
        public float erodeSpeed = 0.3f;

        public float evaporateSpeed = .01f;
        public float gravity = 4;
        public float startSpeed = 1;
        public float startWater = 1;
        [Range(0, 1)] public float inertia = 0.3f;

        // Internal
        float[] map;
        Mesh mesh;
        int borderSize;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public void GenerateHeightMap() 
        {
            borderSize = erosionBrushRadius * 2;
            mapSize -= borderSize;
            map = FindObjectOfType<HeightMapGenerator>().GenerateHeightMap(mapSize + borderSize);
            mapSize += borderSize;
        }

        public void Erode(float[] heightmap = null)
        {
            map = heightmap ?? heightMapGenerator.GenerateHeightMap(mapSize);
            
            int numThreads = numErosionIterations / 1024;

            // Create brush
            List<int> brushIndexOffsets = new List<int>();
            List<float> brushWeights = new List<float>();

            float weightSum = 0;
            for (int brushY = -erosionBrushRadius; brushY <= erosionBrushRadius; brushY++)
            {
                for (int brushX = -erosionBrushRadius; brushX <= erosionBrushRadius; brushX++)
                {
                    float sqrDst = brushX * brushX + brushY * brushY;
                    if (!(sqrDst < erosionBrushRadius * erosionBrushRadius))
                        continue;
                    
                    brushIndexOffsets.Add(brushY * mapSize + brushX);
                    float brushWeight = 1 - Mathf.Sqrt(sqrDst) / erosionBrushRadius;
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
            erosion.SetBuffer(0, "brushIndices", brushIndexBuffer);
            erosion.SetBuffer(0, "brushWeights", brushWeightBuffer);

            // Generate random indices for droplet placement
            int[] randomIndices = new int[numErosionIterations];
            for (int i = 0; i < numErosionIterations; i++)
            {
                int randomX = Random.Range(erosionBrushRadius, mapSize + erosionBrushRadius);
                int randomY = Random.Range(erosionBrushRadius, mapSize + erosionBrushRadius);
                randomIndices[i] = randomY * mapSize + randomX;
                // Task.Delay(100);
            }

            // Send random indices to compute shader
            ComputeBuffer randomIndexBuffer = new ComputeBuffer(randomIndices.Length, sizeof(int));
            randomIndexBuffer.SetData(randomIndices);
            erosion.SetBuffer(0, "randomIndices", randomIndexBuffer);
            
            // Heightmap buffer
            ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));
            mapBuffer.SetData(map);
            erosion.SetBuffer(0, "map", mapBuffer);
            
            // Settings
            erosion.SetInt("borderSize", borderSize);
            erosion.SetInt("mapSize", mapSize);
            erosion.SetInt("brushLength", brushIndexOffsets.Count);
            erosion.SetInt("maxLifetime", maxLifetime);
            erosion.SetFloat("inertia", inertia);
            erosion.SetFloat("sedimentCapacityFactor", sedimentCapacityFactor);
            erosion.SetFloat("minSedimentCapacity", minSedimentCapacity);
            erosion.SetFloat("depositSpeed", depositSpeed);
            erosion.SetFloat("erodeSpeed", erodeSpeed);
            erosion.SetFloat("evaporateSpeed", evaporateSpeed);
            erosion.SetFloat("gravity", gravity);
            erosion.SetFloat("startSpeed", startSpeed);
            erosion.SetFloat("startWater", startWater);

            // Run compute shader
            erosion.Dispatch(0, numThreads, 1, 1);
            mapBuffer.GetData(map);

            // Release buffers
            mapBuffer.Release();
            randomIndexBuffer.Release();
            brushIndexBuffer.Release();
            brushWeightBuffer.Release();
        }

        public void ConstructMesh() => terrainGenerator.GenerateMesh(map);

        void AssignMeshComponents()
        {
            // Ensure mesh renderer and filter components are assigned
            if (!gameObject.GetComponent<MeshFilter>())
                gameObject.AddComponent<MeshFilter>();

            if (!GetComponent<MeshRenderer>())
                gameObject.AddComponent<MeshRenderer>();

            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
        }

        private void OnValidate() => heightMapGenerator.postGenerate.Register(Erode, 4998);
    }
}