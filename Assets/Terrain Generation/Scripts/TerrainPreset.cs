using NoiseGenerator.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace NoiseGenerator.TerrainGeneration
{
	[CreateAssetMenu(fileName = "Terrain Preset", menuName = "Custom/Terrain Preset", order = 0)]
	public class TerrainPreset : HeightmapPreset
	{
		[FormerlySerializedAs("TerrainShaderSettings")] public TerrainSettings TerrainSettings;
	}
}