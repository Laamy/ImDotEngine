using System;

public static class Mathf
{
    public static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static float Abs(float value)
    {
        return (float)Math.Abs(value);
    }

    public static float Sqrt(float value) => (float)System.Math.Sqrt((float)value);
    public static float Floor(float value) => (float)System.Math.Floor((float)value);

    public static float Lerp(this float firstFloat, float secondFloat, float by)
    {
        return firstFloat * (1 - by) + secondFloat * by;
    }

    public static float Pow(double x, double y) => (float)System.Math.Pow(x, y);

    public static float Max(float x, float y)
    {
        if (x > y) return x;
        return y;
    }

    public static float Min(float x, float y)
    {
        if (x < y) return x;
        return y;
    }
}