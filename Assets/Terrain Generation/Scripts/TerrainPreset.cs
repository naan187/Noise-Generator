using NoiseGenerator.Core;
using UnityEngine;

namespace NoiseGenerator.TerrainGeneration
{
	[CreateAssetMenu(fileName = "Terrain Preset", menuName = "Custom/Terrain Preset")]
	public class TerrainPreset : HeightmapPreset
	{
		public TerrainSettings TerrainSettings;
	}
}