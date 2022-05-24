using UnityEngine;

namespace NoiseGenerator.Core
{
    [CreateAssetMenu(fileName = "Preset", menuName = "Custom/Preset", order = 0)]
    public class Preset : ScriptableObject
    {
        public NoiseSettings NoiseSettings = new (200);
    }
}