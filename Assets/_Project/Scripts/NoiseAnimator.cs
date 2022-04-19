using UnityEngine;

namespace NoiseGenerator
{
    [ExecuteInEditMode]
    public class NoiseAnimator : MonoBehaviour
    {
        [SerializeField] private NoiseMapGenerator _noiseMapGenerator;
        [SerializeField] private bool _animate;
        [SerializeField] private float _speed;

        private void Update()
        {
            if (!_animate) return;

            ref var value = ref _noiseMapGenerator.NoiseSettings.f;

            value += Time.deltaTime * _speed;

            if (_noiseMapGenerator.AutoGenerate)
                _noiseMapGenerator.Generate();
        }
    }
}
