using System;

public class SimplexPerlin
{
    private int _seed;
    private readonly int[] _perm;

    public int Seed
    {
        get => _seed;
        set
        {
            _seed = value;
            Random rand = new Random(_seed);
            for (int i = 0; i < 256; i++) _perm[i] = rand.Next(256);
            for (int i = 0; i < 256; i++) _perm[256 + i] = _perm[i];
        }
    }

    public SimplexPerlin(int seed)
    {
        _perm = new int[512];
        Seed = seed;
    }

    public float GetValue(float x, float y)
    {
        int X = (int)Mathf.Floor(x) & 255,
            Y = (int)Mathf.Floor(y) & 255;

        x -= (float)Mathf.Floor(x);
        y -= (float)Mathf.Floor(y);

        float u = Fade(x),
              v = Fade(y);

        int aa = _perm[X] + Y;
        int ab = _perm[X + 1] + Y;
        int ba = _perm[X] + Y + 1;
        int bb = _perm[X + 1] + Y + 1;

        float gradAA = Grad(_perm[aa], x, y);
        float gradAB = Grad(_perm[ab], x - 1, y);
        float gradBA = Grad(_perm[ba], x, y - 1);
        float gradBB = Grad(_perm[bb], x - 1, y - 1);

        float lerpX1 = Lerp(gradAA, gradAB, u);
        float lerpX2 = Lerp(gradBA, gradBB, u);

        return Lerp(lerpX1, lerpX2, v);
    }

    private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
    private static float Lerp(float a, float b, float t) => a + t * (b - a);

    private static float Grad(int hash, float x, float y)
    {
        int h = hash & 7;
        float u = h < 4 ? x : y;
        float v = h < 4 ? y : x;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}