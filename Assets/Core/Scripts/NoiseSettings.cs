using System;
using UnityEngine;

namespace NoiseGenerator.Core
{
	[Serializable]
	public class NoiseSettings
	{
		public int Seed;

		[Range(16, 256)]
		public int Size = 200;

		public Vector2 Offset;

		[Min(.75f)]
		public float Scale = 50;

		public int OctaveAmount
		{
			get => Octaves.OctaveAmount;
			set => Octaves.OctaveAmount = value;
		}

		public OctaveList Octaves = new (4);

		[Range(0f, 1f)]
		public float Persistence = .5f;
		public float Lacunarity = 2;

		public AnimationCurve HeightCurve = AnimationCurve.Linear(0, 0, 1, 1);

		public NoiseSettings() { }

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