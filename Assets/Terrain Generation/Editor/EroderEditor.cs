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

using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace NoiseGenerator.TerrainGeneration
{
    [CustomEditor(typeof(Eroder))]
    public class EroderEditor : UnityEditor.Editor
    {
        Eroder eroder;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate Mesh"))
            {
                eroder.GenerateHeightMap();
                eroder.ConstructMesh();
            }

            string numIterationsString = eroder.numErosionIterations.ToString();
            if (eroder.numErosionIterations >= 1000)
                numIterationsString = eroder.numErosionIterations / 1000 + "k";

            if (GUILayout.Button("Erode (" + numIterationsString + " iterations)"))
            {
                var sw = new Stopwatch();

                sw.Start();
                eroder.GenerateHeightMap();
                int heightMapTimer = (int) sw.ElapsedMilliseconds;
                sw.Reset();

                sw.Start();
                eroder.Erode();
                int erosionTimer = (int) sw.ElapsedMilliseconds;
                sw.Reset();

                sw.Start();
                eroder.ConstructMesh();
                int meshTimer = (int) sw.ElapsedMilliseconds;

                if (eroder.printTimers)
                {
                    Debug.Log($"{eroder.mapSize}x{eroder.mapSize} heightmap generated in {heightMapTimer}ms");
                    Debug.Log($"{numIterationsString} erosion iterations completed in {erosionTimer}ms");
                    Debug.Log($"Mesh constructed in {meshTimer}ms");
                }

            }
        }

        void OnEnable()
        {
            eroder = (Eroder) target;
            Tools.hidden = true;
        }

        void OnDisable()
        {
            Tools.hidden = false;
        }
    }

}