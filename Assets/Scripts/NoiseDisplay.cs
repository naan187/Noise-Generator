using UnityEngine;
using UnityEngine.Serialization;

public class NoiseDisplay : MonoBehaviour
{
    [SerializeField] private Renderer _textureRenderer;
    [SerializeField] private bool _sampleFromCustomGradient;
    [SerializeField] private bool _invertNoise;
    [SerializeField] private Gradient _noiseGradient;

    public void UpdateTex(float[,] noiseVal, NoiseMapGenerator.NoiseSettings noiseSettings)
    {
        int width = noiseVal.GetLength(0);
        int height = noiseVal.GetLength(1);

        Texture2D tex = new Texture2D(noiseSettings.Width, noiseSettings.Height);
        Color[] texColors = new Color[noiseSettings.Width * noiseSettings.Height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                float v = _invertNoise ? Mathf.Abs(1 - noiseVal[x, y]) : noiseVal[x, y];

                if (_sampleFromCustomGradient)
                    texColors[y * width + x] = _noiseGradient.Evaluate(v);
                else
                    texColors[y * width + x] = Color.Lerp(Color.black, Color.white, v);
            }
        }

        tex.SetPixels(texColors);
        tex.Apply();

        _textureRenderer.sharedMaterial.mainTexture = tex;
        _textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }

    private void OnValidate()
    {
        if (GetComponent<NoiseMapGenerator>().AutoUpdate) NoiseMapGenerator.onGenerate?.Invoke();
    }
}