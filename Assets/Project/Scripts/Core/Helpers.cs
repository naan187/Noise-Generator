using System;

namespace NoiseGenerator.Core
{
    public static class Helpers
    {
        public static void IteratePointsOnMap(int size, Action<int, int> action)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    action(x, y);
                }
            }
        }

    }
}