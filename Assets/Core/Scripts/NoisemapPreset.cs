using UnityEngine;

namespace NoiseGenerator.Core
{
    [CreateAssetMenu(fileName = "Noisemap Preset", menuName = "Custom/Noisemap Preset", order = 0)]
    public class NoisemapPreset : ScriptableObject
    {
        public NoiseSettings NoiseSettings = new ();
    }
}