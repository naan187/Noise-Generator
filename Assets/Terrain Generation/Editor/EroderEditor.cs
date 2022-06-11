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
    [CustomEditor(typeof(Erosion))]
    public class EroderEditor : UnityEditor.Editor
    {
        Erosion _Erosion;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate Mesh"))
            {
                _Erosion.GenerateHeightMap();
                _Erosion.ConstructMesh();
            }

            string numIterationsString = _Erosion.numErosionIterations.ToString();
            if (_Erosion.numErosionIterations >= 1000)
                numIterationsString = _Erosion.numErosionIterations / 1000 + "k";

            if (GUILayout.Button("Erode (" + numIterationsString + " iterations)"))
            {
                var sw = new Stopwatch();

                sw.Start();
                _Erosion.GenerateHeightMap();
                int heightMapTimer = (int) sw.ElapsedMilliseconds;
                sw.Reset();

                sw.Start();
                _Erosion.Erode();
                int erosionTimer = (int) sw.ElapsedMilliseconds;
                sw.Reset();

                sw.Start();
                _Erosion.ConstructMesh();
                int meshTimer = (int) sw.ElapsedMilliseconds;

                if (_Erosion.printTimers)
                {
                    Debug.LogFormat("{0}x{0} heightmap generated in {1}ms", _Erosion.mapSize, heightMapTimer);
                    Debug.LogFormat("{0} erosion iterations completed in {1}ms", numIterationsString, erosionTimer);
                    Debug.LogFormat("Mesh constructed in {0}ms", meshTimer);
                }

            }
        }

        void OnEnable()
        {
            _Erosion = (Erosion) target;
            Tools.hidden = true;
        }

        void OnDisable()
        {
            Tools.hidden = false;
        }
    }

}