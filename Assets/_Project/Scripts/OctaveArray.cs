using System;
using System.Collections;
using UnityEngine;

namespace NoiseGenerator
{
    [Serializable]
    public class OctaveArray : IEnumerable
    {
        [Range(1, 8)] public int OctaveAmount = 1;
        [SerializeField] private bool _EditOctaves;
        [SerializeField] private Octave[] _Octaves;

        public int Length => OctaveAmount;

        public Octave this[int i]
        {
            get => _Octaves[i];
            set
            {
                _Octaves[i] = value;
                OctaveAmount = _Octaves.Length;
            }
        }

        public OctaveArray()
        {
            _Octaves = new Octave[] { new () };
        }

        public OctaveArray(int length)
        {
            _Octaves = new Octave[length];
        }

        public IEnumerator GetEnumerator() => _Octaves.GetEnumerator();
    }
}
