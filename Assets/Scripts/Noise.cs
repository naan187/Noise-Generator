using UnityEngine;

public static class Noise
{
    public static float Evaluate(Vector2 p) => Mathf.PerlinNoise(p.x, p.y);

    public static float Warp(Vector2 p)
    {
        Vector2 q = new Vector2(Evaluate(p), Evaluate(p + new Vector2(5.2f, 1.3f)));

        Vector2 r = new Vector2(
            Evaluate(p + (4f) * q + new Vector2(1.7f,9.2f)),
            Evaluate(p + (4f) *q + new Vector2(8.3f,2.8f))
        );

        return Evaluate(p + 4f * q);
    }
}
