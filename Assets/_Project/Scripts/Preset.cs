using UnityEngine;

namespace NoiseGenerator
{
    [CreateAssetMenu(fileName = "Preset", menuName = "Custom/Preset", order = 0)]
    public class Preset : ScriptableObject
    {
        public NoiseSettings NoiseSettings;
    }
}