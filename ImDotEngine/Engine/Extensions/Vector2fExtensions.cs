using SFML.System;

public static class Vector2fExtensions
{
    public static Vector2f Normalize(this Vector2f vector)
    {
        float length = (float)Mathf.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        return length > 0 ? new Vector2f(vector.X / length, vector.Y / length) : new Vector2f(0, 0);
    }

    public static float Magnitude(this Vector2f vector)
    {
        return (float)Mathf.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
    }

    public static Vector2f Clamp(this Vector2f vector, Vector2f min, Vector2f max)
    {
        float clampedX = Mathf.Clamp(vector.X, min.X, max.X);
        float clampedY = Mathf.Clamp(vector.Y, min.Y, max.Y);
        return new Vector2f(clampedX, clampedY);
    }

    public static Vector2f Lerp(this Vector2f firstVector, Vector2f secondVector, float by)
    {
        float retX = Mathf.Lerp(firstVector.X, secondVector.X, by);
        float retY = Mathf.Lerp(firstVector.Y, secondVector.Y, by);
        return new Vector2f(retX, retY);
    }

    // turns out C# doesnt let you overload operators in extensions..

    public static Vector2f Div(this Vector2f vector1, Vector2f vector2)
    {
        Vector2f result = vector1;

        result.X /= vector2.X;
        result.Y /= vector2.Y;

        return result;
    }

    public static Vector2f Div(this Vector2f vector1, float scale)
    {
        Vector2f result = vector1;

        result.X /= scale;
        result.Y /= scale;

        return result;
    }

    public static Vector2f Mul(this Vector2f vector1, Vector2f vector2)
    {
        Vector2f result = vector1;

        result.X *= vector2.X;
        result.Y *= vector2.Y;

        return result;
    }

    public static Vector2f Mul(this Vector2f vector1, float scale)
    {
        Vector2f result = vector1;

        result.X *= scale;
        result.Y *= scale;

        return result;
    }
}