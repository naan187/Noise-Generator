using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseGenerator.Core
{
    public class OctaveList : IEnumerable
    {
        [Min(1)] public int OctaveAmount = 1;
        private List<Octave> _Octaves;
        
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

        public OctaveList() => _Octaves = new List<Octave>(8);

        public OctaveList(int length)
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

            if (newLength > length)
                for (int i = length; i < newLength; i++)
                    result.Add(new Octave());
            else
                for (int i = length; i > newLength; i--)
                    result.Remove(result[^1]);

            _Octaves = result;
        }
    }
}
