using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator.TerrainGeneration
{
	[System.Serializable]
	public class TerrainShaderSettings
	{
		[FormerlySerializedAs("HeightBasedTerrainGradient")] public Gradient ColorGradient;
		public Color SteepTerrainColor;
		[Range(0f, 1f)]
		public float SteepnessThreshold;
		[FormerlySerializedAs("Sharpness")] [Range(0f, 1f)]
		public float BlendAmount;
	}
}