namespace NoiseGenerator
{
    public class MinMax
    {
        public float Max = float.MinValue;
        public float Min = float.MaxValue;

        public void Update(float v)
        {
            Max = v > Max ? v : Max;
            Min = v < Min ? v : Min;
        }
    }
}