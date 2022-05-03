using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGenerator
{
    [Serializable]
    public class OctaveArray : IEnumerable
    {
        [Range(1, 8)] public int OctaveAmount = 1;
        [SerializeField] private List<Octave> _Octaves;
        
        public int length => _Octaves.Count;

        public Octave this[int i]
        {
            get => _Octaves[i];
            set
            {
                _Octaves[i] = value;
                OctaveAmount = _Octaves.Count;
            }
        }

        public OctaveArray() => _Octaves = new List<Octave>(8);

        public OctaveArray(int length)
        {
            _Octaves = new List<Octave>(8);
            OctaveAmount = length;
            
            for (int i = 0; i < length; i++)
                _Octaves.Add(new Octave());
        }

        public IEnumerator GetEnumerator() => _Octaves.GetEnumerator();

        public void Resize(int newLength)
        {
            if (newLength == length)
                return;
            
            var result = _Octaves;

            bool check = newLength > length;

            if (check)
                for (int i = length; i < newLength; i++)
                    result.Add(new Octave());
            else
                for (int i = length; i > newLength; i--)
                    result.Remove(result[^1]);

            _Octaves = result;
        }
    }
}
