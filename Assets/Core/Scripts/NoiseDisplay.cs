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

        public void UpdateTex(float[,] noiseVal)
        {
            int size = noiseVal.GetLength(0);

            Texture2D tex = new Texture2D(size, size);

            Color[] texColors = new Color[size * size];

            float v;

            Helpers.IteratePointsOnMap(size, (x, y) => {
                v = _InvertNoise ? Mathf.Abs(1 - noiseVal[x, y]) : noiseVal[x, y];
                if (v is float.NaN)
                    v = 0;

                if (_SampleFromCustomGradient)
                    texColors[y * size + x] = _NoiseGradient.Evaluate(v);
                else
                    texColors[y * size + x] = Color.Lerp(Color.black, Color.white, v);
            });

            tex.filterMode = _FilterMode;

            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels(texColors);
            tex.Apply();

            _TextureRenderer.sharedMaterial.mainTexture = tex;
            _TextureRenderer.transform.localScale = new Vector3(size * .1f, 1, size * .1f);
        }

        public void OnValidate()
        {
            _HeightMapGenerator ??= GetComponent<HeightMapGenerator>();
            
            _HeightMapGenerator.postGenerate.Register(UpdateTex, _Priority);
            
            if (_HeightMapGenerator.AutoGenerate) _HeightMapGenerator.Generate();
        }
    }
}