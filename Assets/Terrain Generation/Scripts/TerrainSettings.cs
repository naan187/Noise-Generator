using UnityEngine;

namespace NoiseGenerator.TerrainGeneration
{
	[System.Serializable]
	public struct TerrainSettings
	{
		public float HeightMultiplier;
		public Color SteepTerrainColor;
		[Range(0f, 1f)]
		public float SteepnessThreshold;
		[Range(0f, 1f)]
		public float Sharpness;

 
		public GradientBased GradientBasedSettings;
		public IndividualValues IndividualValuesSettings;

		[System.Serializable]
		public struct GradientBased
		{
			public Gradient ColorGradient;
		}

		[System.Serializable]
		public struct IndividualValues
		{
			public Color GrassColor;
			public Color SnowColor;
			[Range(0f, 1f)]
			public float MaxGrassHeight;
			[Range(0f, 1f)]
			public float MinSnowHeight;
		}

		public enum WorkflowModes
		{
			GradientBased = 0,
			IndividualValues = 1
		}
		
		public TerrainSettings(TerrainSettings source)
		{
			HeightMultiplier = source.HeightMultiplier;
			SteepTerrainColor = source.SteepTerrainColor;
			SteepnessThreshold = source.SteepnessThreshold;
			Sharpness = source.Sharpness;

			{
				GradientBasedSettings.ColorGradient = source.GradientBasedSettings.ColorGradient;
			}

			{
				IndividualValuesSettings.GrassColor = source.IndividualValuesSettings.GrassColor;
				IndividualValuesSettings.SnowColor = source.IndividualValuesSettings.SnowColor;
				IndividualValuesSettings.MaxGrassHeight = source.IndividualValuesSettings.MaxGrassHeight;
				IndividualValuesSettings.MinSnowHeight = source.IndividualValuesSettings.MinSnowHeight;
			}
		}
	}
}