using UnityEngine;

namespace NoiseGenerator.TerrainGeneration
{
	[System.Serializable]
	public class TerrainShaderSettings
	{
		public Color SteepTerrainColor;
		[Range(0f, 1f)]
		public float SteepnessThreshold;
		[Range(0f, 1f)]
		public float Sharpness;

		
		public GradientBased GradientBasedSettings;
		public IndividualValues IndividualValuesSettings;

		[System.Serializable]
		public class GradientBased
		{
			public Gradient ColorGradient;
		}
		
		[System.Serializable]
		public class IndividualValues
		{
			public Color GrassColor = new (39, 114, 33);
			public Color SnowColor = Color.white;
			[Range(0f, 1f)]
			public float MaxGrassHeight;
			[Range(0f, 1f)]
			public float MinSnowHeight;
			[Range(0f, 1f)]
			public float BlendDst;
		}
		
		public enum WorkflowModes
		{
			GradientBased = 0,
			IndividualValues = 1
		}
	}
}