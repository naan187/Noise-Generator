using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGenerator.Core
{
    internal struct OctaveList : IEnumerable
    {
        [SerializeField, Range(1, 8)] 
        private int _OctaveAmount;
        public int OctaveAmount { get => _OctaveAmount; set { _OctaveAmount = value; Resize(value); } }
        private List<Octave> _Octaves;
        
        public int Length => _Octaves.Count;

        public Octave this[int i]
        {
            get => _Octaves[i];
            set
            {
                _Octaves[i] = value;
                OctaveAmount = _Octaves.Count;
            }
        }

        public OctaveList(int numOctaves = 4)
        {
            _Octaves = new List<Octave>(8);
            _OctaveAmount = numOctaves;
            
            for (int i = 0; i < numOctaves; i++)
                _Octaves.Add(new Octave());
        }

        public IEnumerator GetEnumerator() => _Octaves.GetEnumerator();

        public void Resize(int newLength)
        {
            if (_Octaves is null)
                _Octaves = new List<Octave>(8);
            else if (newLength == Length)
                return;
    
            var result = _Octaves;

            if (newLength > Length)
                for (int i = Length; i < newLength; i++)
                    result.Add(new Octave());
            else
                for (int i = Length; i > newLength; i--)
                    result.Remove(result[^1]);

            _Octaves = result;
        }
    }
}
