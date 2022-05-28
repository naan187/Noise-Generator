using System;
using System.Collections.Generic;
using System.Linq;

namespace NoiseGenerator.Core
{
    public class PostGenerateEvent
    {
        private Dictionary<int, Action<float[,]>> _PostGenerateWithHeightmap = new ();
        private Dictionary<int, Action> _PostGenerate = new ();

        //integer is the priority, byte is the action type(0 if takes in nothing, 1 if taking in heightmap)
        private Dictionary<int, byte> _Priorities = new ();

        public void Register(Action action, int priority)
        {
            if (_PostGenerate.ContainsKey(-priority) || _PostGenerateWithHeightmap.ContainsKey(-priority))
                return;

            _PostGenerate.Add(-priority, action);
            _Priorities.Add(-priority, 0);
        }

        public void Register(Action<float[,]> action, int priority)
        {
            if (_PostGenerateWithHeightmap.ContainsKey(-priority) || _PostGenerate.ContainsKey(-priority))
                return;

            _PostGenerateWithHeightmap.Add(-priority, action);
            _Priorities.Add(-priority, 1);
        }

        public void RemoveListener(Action<float[,]> action)
        {
            var elementKey = _PostGenerateWithHeightmap.FirstOrDefault(a => a.Value == action).Key;
            _PostGenerateWithHeightmap.Remove(elementKey);
        }

        public void RemoveListener(Action action)
        {
            var elementKey = _PostGenerate.FirstOrDefault(a => a.Value == action).Key;
            _PostGenerate.Remove(elementKey);
        }

        public void Invoke(float[,] heightmap)
        {
            var sortedPriorities =
                from entry in _Priorities
                orderby entry.Key select entry;

            foreach (var (priority, actionType) in sortedPriorities)
            {
                if (actionType == 0)
                    _PostGenerate[priority]?.Invoke();
                else
                    _PostGenerateWithHeightmap[priority]?.Invoke(heightmap);
            }
        }
    }
}