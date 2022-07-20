using UnityEngine;

namespace NoiseGenerator.Core
{
    [RequireComponent(typeof(Renderer))]
    public class NoiseDisplay : MonoBehaviour
    {
        [SerializeField]
        private Renderer _TextureRenderer;
        [SerializeField]
        private FilterMode _FilterMode;
        [SerializeField]
        private bool _SampleFromCustomGradient;
        [SerializeField]
        private bool _InvertNoise;
        [SerializeField]
        private Gradient _NoiseGradient;
        
        private const int _Priority = 4998;

        private HeightMapGenerator _HeightMapGenerator;

        public void UpdateTex(float[] noiseVal)
        {
            int size = (int) Mathf.Sqrt(noiseVal.Length);

            Texture2D tex = new Texture2D(size, size);

            Color[] texColors = new Color[size * size];

            float v;
            
            for (int i = 0; i < size*size; i++) {
                v = _InvertNoise ? Mathf.Abs(1 - noiseVal[i]) : noiseVal[i];
                if (v is float.NaN)
                    v = 0;

                if (_SampleFromCustomGradient)
                    texColors[i] = _NoiseGradient.Evaluate(v);
                else
                    texColors[i] = Color.Lerp(Color.black, Color.white, v);
            }


            tex.filterMode = _FilterMode;

            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels(texColors);
            tex.Apply();

            _TextureRenderer.sharedMaterial.mainTexture = tex;
        }

        public void OnValidate()
        {
            _HeightMapGenerator ??= GetComponent<HeightMapGenerator>();
            
            _HeightMapGenerator.postGenerate.Register(UpdateTex, _Priority);
            
            if (_HeightMapGenerator.AutoGenerate) _HeightMapGenerator.Generate(_HeightMapGenerator.UseComputeShader);
        }
    }
}