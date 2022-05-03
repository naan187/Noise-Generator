using System;

namespace NoiseGenerator
{
    public static class Helpers
    {
        public static void IteratePointsOnMap(int width, int height, Action<int, int> action)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    action(x, y);
                }
            }
        }

    }
}