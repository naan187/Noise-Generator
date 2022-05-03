using UnityEngine;

namespace NoiseGenerator
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
        
        private HeightMapGenerator _HeightMapGenerator;

        public void UpdateTex(float[,] noiseVal)
        {
            int width = noiseVal.GetLength(0);
            int height = noiseVal.GetLength(1);

            Texture2D tex = new Texture2D(width, height);

            Color[] texColors = new Color[width * height];

            float v;

            Helpers.IteratePointsOnMap(width, height, (x, y) => {
                v = _InvertNoise ? Mathf.Abs(1 - noiseVal[x, y]) : noiseVal[x, y];

                if (_SampleFromCustomGradient)
                    texColors[y * width + x] = _NoiseGradient.Evaluate(v);
                else
                    texColors[y * width + x] = Color.Lerp(Color.black, Color.white, v);
            });

            tex.filterMode = _FilterMode;

            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels(texColors);
            tex.Apply();

            _TextureRenderer.sharedMaterial.mainTexture = tex;
            _TextureRenderer.transform.localScale = new Vector3(width, 1, height);
        }

        public void OnValidate()
        {
            _HeightMapGenerator ??= GetComponent<HeightMapGenerator>();
            
            if (_HeightMapGenerator.AutoGenerate) _HeightMapGenerator.Generate();
        }
    }
}