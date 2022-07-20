using System;
using UnityEngine;

namespace NoiseGenerator.Core
{
	[Serializable]
	public struct NoiseSettings
	{
		public int Seed;

		[Min(16)]
		public int Size;

		public Vector2 Offset;

		[Min(.75f)]
		public float Scale;

		public int OctaveAmount
		{
			get => Octaves.OctaveAmount;
			set => Octaves.OctaveAmount = value;
		}

		internal OctaveList Octaves;

		[Range(0f, 1f)]
		public float Persistence;
		public float Lacunarity;

		public AnimationCurve HeightCurve;

		public NoiseSettings(int numOctaves = 4)
		{
			Seed = 0;
			Size = 128;
			Offset = Vector2.zero;
			Scale = 50;
			Octaves = new OctaveList(numOctaves);
			Persistence = .5f;
			Lacunarity = 2f;
			HeightCurve = AnimationCurve.Linear(0, 0, 1, 1);
		}

		public NoiseSettings(NoiseSettings source)
		{
			Seed = source.Seed;
			Size = source.Size;
			Offset = source.Offset;
			Scale = source.Scale;
			Octaves = source.Octaves;
			Persistence = source.Persistence;
			Lacunarity = source.Lacunarity;
			HeightCurve = source.HeightCurve;
		}
	}
}