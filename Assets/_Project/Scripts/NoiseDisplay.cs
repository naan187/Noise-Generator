using System;
using UnityEngine;

namespace NoiseGenerator
{
    [RequireComponent(typeof(Renderer))]
    public class NoiseDisplay : MonoBehaviour
    {
        [SerializeField] private Renderer _TextureRenderer;
        [SerializeField] private bool _SampleFromCustomGradient;
        [SerializeField] private bool _InvertNoise;
        [SerializeField] private Gradient _NoiseGradient;
        
        private HeightMapGenerator _HeightMapGenerator;

        public void UpdateTex(float[,] noiseVal, NoiseSettings noiseSettings)
        {
            int width = noiseVal.GetLength(0);
            int height = noiseVal.GetLength(1);

            Texture2D tex = new Texture2D(noiseSettings.Width, noiseSettings.Height);
            Color[] texColors = new Color[noiseSettings.Width * noiseSettings.Height];

            float v;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    v = _InvertNoise ? Mathf.Abs(1 - noiseVal[x, y]) : noiseVal[x, y];

                    if (_SampleFromCustomGradient)
                        texColors[y * width + x] = _NoiseGradient.Evaluate(v);
                    else
                        texColors[y * width + x] = Color.Lerp(Color.black, Color.white, v);
                }
            }

            tex.SetPixels(texColors);
            tex.Apply();

            _TextureRenderer.sharedMaterial.mainTexture = tex;
            _TextureRenderer.transform.localScale = new Vector3(width, 1, height);
        }

        private void OnValidate()
        {
            _HeightMapGenerator ??= GetComponent<HeightMapGenerator>();
            
            if (_HeightMapGenerator.AutoGenerate) _HeightMapGenerator.Generate();
        }
    }
}