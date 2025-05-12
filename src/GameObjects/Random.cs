using System;

namespace src.GameObjects;
public class Rng
{
    // Private fields:
    private static readonly Random random = new();
    
    // Public methods:
    public static float NextFloat() => (float)random.NextDouble();
    public static float NextFloat(float max) => ((float)random.NextDouble()) * max;
    public static float NextFloat(float min, float max) => ((float)random.NextDouble()) * (max - min) + min;

    public static bool NextBool() => random.NextDouble() < 0.5f;
    public static bool NextBool(float probability) => random.NextDouble() < probability;

    public static int NextInt() => random.Next();
    public static int NextInt(int max) => random.Next(max);
    public static int NextInt(int min, int max) => random.Next(min, max);
}

