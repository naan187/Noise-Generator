using UnityEngine;

namespace NoiseGenerator.Core
{
    [CreateAssetMenu(fileName = "Heightmap Preset", menuName = "Custom/Heightmap Preset", order = 0)]
    public class HeightmapPreset : ScriptableObject
    {
        public NoiseSettings NoiseSettings = new (4);
    }
}