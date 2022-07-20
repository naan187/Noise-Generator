using UnityEngine;

namespace NoiseGenerator.Core
{
    [CreateAssetMenu(fileName = "Noisemap Preset", menuName = "Custom/Noisemap Preset", order = 0)]
    public class HeightmapPreset : ScriptableObject
    {
        public NoiseSettings NoiseSettings = new (4);
    }
}