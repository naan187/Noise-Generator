using System;
using UnityEngine;

namespace NoiseGenerator.Core
{
	public class NoiseAnimator : MonoBehaviour
	{ 
		[SerializeField]
		private HeightMapGenerator _HeightMapGenerator;
		[SerializeField]
		private float _Speed;

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void Update()
		{
			if (_HeightMapGenerator is null)
				return;

			_HeightMapGenerator.NoiseSettings.Offset.x += Time.deltaTime * _Speed;
			if (_HeightMapGenerator.AutoGenerate)
				_HeightMapGenerator.Generate(_HeightMapGenerator.UseComputeShader);
		}
	}
}