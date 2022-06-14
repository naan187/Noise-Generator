using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator.TerrainGeneration
{
	[System.Serializable]
	public class TerrainShaderSettings
	{
		public Gradient ColorGradient;
		public Color SteepTerrainColor;
		[Range(0f, 1f)]
		public float SteepnessThreshold;
		[Range(0f, 1f)]
		public float Sharpness;
	}
}