using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator
{
    public class NoiseAnimator : MonoBehaviour
    {
        [FormerlySerializedAs("HeightMapGenerator")] [SerializeField] private HeightMapGenerator _HeightMapGenerator;
        [FormerlySerializedAs("_animate")] [SerializeField] private bool _Animate;
        [FormerlySerializedAs("_speed")] [SerializeField] private float _Speed;

        private bool _isAnimating;

        private void Animate()
        {
            if (!_Animate) return;

            ref var value = ref _HeightMapGenerator.NoiseSettings.f;
            
            value += Time.deltaTime * _Speed;

            if (_HeightMapGenerator.AutoGenerate)
                _HeightMapGenerator.Generate();
        }

        private void OnValidate()
        {
            //TODO: fix this mess
            if (_Animate && !_isAnimating)
            {
                InvokeRepeating(nameof(Animate), .5f, .1f);
                _isAnimating = true;
                return;
            }
            else if (!_Animate && _isAnimating)
                return;
            
            CancelInvoke(nameof(Animate));
        }
    }
}
