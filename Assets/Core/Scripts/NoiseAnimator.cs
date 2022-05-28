using System;
using UnityEngine;

namespace NoiseGenerator.Core
{
	public class NoiseAnimator : MonoBehaviour
	{
		[SerializeField] private HeightMapGenerator _HeightMapGenerator;
		[SerializeField] private float _Speed;
		
		
		private void Update()
		{
			_HeightMapGenerator.NoiseSettings.f += Time.deltaTime * _Speed;
			if (_HeightMapGenerator.AutoGenerate)
				_HeightMapGenerator.Generate();
		}
	}
}