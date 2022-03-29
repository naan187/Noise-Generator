using UnityEngine;

//TODO: Make the animator accept any value of type float to animate
namespace NoiseGenerator
{
    [ExecuteInEditMode]
    public class NoiseAnimator : MonoBehaviour
    {
        [SerializeField] private NoiseMapGenerator _noiseMapGenerator;
        [SerializeField] private bool _animate;


        private void Update()
        {
            if (!_animate) return;

            _noiseMapGenerator.NoiseSettings._warpSettings.f += Time.deltaTime / 2;

            if (_noiseMapGenerator.AutoUpdate)
                NoiseMapGenerator.onGenerate?.Invoke();
        }
    }
}
