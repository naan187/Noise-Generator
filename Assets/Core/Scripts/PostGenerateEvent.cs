using System;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using UnityEngine;
using UnityEngine.Events;

namespace NoiseGenerator.Core
{
    public class PostGenerateEvent
    {
        private readonly Dictionary<int, Action> _Voids = new ();
        private readonly Dictionary<int, Action<float[]>> _VoidsTakingInHeightmaps = new ();
        private readonly Dictionary<int, Func<float[], float[]>> _PostProcessing = new ();

        private readonly Dictionary<int, ActionType> _Priorities = new ();


        public void Register(Action action, int priority)
        {
            if (ContainsKey(-priority))
                return;

            _Voids.Add(-priority, action);
            _Priorities.Add(-priority, ActionType.Void);
        }

        public void Register(Action<float[]> action, int priority)
        {
            if (ContainsKey(-priority))
                return;

            _VoidsTakingInHeightmaps.Add(-priority, action);
            _Priorities.Add(-priority, ActionType.VoidTakingInHeightmap);
        }

        public void Register(Func<float[], float[]> action, int priority)
        {
            if (ContainsKey(-priority))
                return;

            _PostProcessing.Add(-priority, action);
            _Priorities.Add(-priority, ActionType.PostProcessing);
        }

        private bool ContainsKey(int key)
        {
            return _Voids.ContainsKey(key)
                   || _VoidsTakingInHeightmaps.ContainsKey(key)
                   || _PostProcessing.ContainsKey(key);
        }

        /*public void RemoveListener(Action<float[]> action)
        {
            var elementKey =
                _PostGenerateWithHeightmap
                    .FirstOrDefault(a => a.Value == action).Key;
            
            _PostGenerateWithHeightmap.Remove(elementKey);
        }

        public void RemoveListener(Action action)
        {
            var elementKey =
                _PostGenerate.FirstOrDefault(a => a.Value == action).Key;
            
            _PostGenerate.Remove(elementKey);
        }*/

        public void Invoke(float[] heightmap)
        {
            var sortedPriorities =
                from entry in _Priorities
                orderby entry.Key select entry;

            foreach (var (priority, actionType) in sortedPriorities)
            {
                switch (actionType)
                {
                    case ActionType.Void:
                        _Voids[priority]?.Invoke();
                        break;
                    case ActionType.VoidTakingInHeightmap:
                        _VoidsTakingInHeightmaps[priority]?.Invoke(heightmap);
                        break;
                    case ActionType.PostProcessing:
                        heightmap = _PostProcessing[priority]?.Invoke(heightmap);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private enum ActionType
        {
            Void = 0,
            VoidTakingInHeightmap = 1,
            PostProcessing = 3
        }
    }
}