using UnityEngine;

namespace NoiseGenerator.TerrainGeneration
{
    public class DayNightCycle : MonoBehaviour
    {
        [SerializeField]
        private float _CycleSpeed;
        [SerializeField]
        private Transform _SunLightParent;

        private void Update() => _SunLightParent.Rotate(-(Time.deltaTime * _CycleSpeed), 0f, 0f);
    }
}
