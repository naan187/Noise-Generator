using UnityEngine;

namespace NoiseGenerator.TerrainGeneration
{
	[System.Serializable]
	public class TerrainSettings
	{
		public float HeightMultiplier;
		[Header("Shading")]
		public Gradient HeightBasedTerrainGradient;
		public Color SteepTerrainColor;
		[Range(0f, 1f)]
		public float SteepnessThreshold;
		[Range(0f, 1f)]
		public float Sharpness;
	}
}