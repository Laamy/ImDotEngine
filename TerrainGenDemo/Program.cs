using System;

internal class Program
{
    static void Main()
    {
        int width = 24*5;
        int height = 24;
        string[][] world = GenerateWorld(width, height);

        // Display the generated world (for testing purposes)
        DisplayWorld(world);
        Console.ReadKey();
    }

    static string[][] GenerateWorld(int width, int height)
    {
        string[][] world = new string[height][];
        SimplexPerlin elevationPerlin = new SimplexPerlin(new Random().Next(0, 1000000));
        SimplexPerlin temperaturePerlin = new SimplexPerlin(new Random().Next(0, 1000000));
        SimplexPerlin humidityPerlin = new SimplexPerlin(new Random().Next(0, 1000000));

        for (int y = 0; y < height; y++)
        {
            world[y] = new string[width];
            for (int x = 0; x < width; x++)
            {
                double tempatureValue = temperaturePerlin.GetValue((float)(x * 0.02), 0);
                double humidityValue = humidityPerlin.GetValue((float)(x * 0.02), 0);

                string biome = DetermineBiome(tempatureValue, humidityValue);

                double elevationValue = elevationPerlin.GetValue((float)(x * 0.06), (float)(y * 0.06));

                int terrainHeight = (int)(5 + (elevationValue * (biome == "M" ? 6 : 2))); // Adjust the parameters for terrain height

                if (y > terrainHeight)
                {
                    world[y][x] = biome; // Terrain
                }
                else
                {
                    world[y][x] = " "; // Empty space
                }
            }
        }

        return world;
    }

    private static string DetermineBiome(double tempatureValue, double humidityValue)
    {
        tempatureValue = (tempatureValue + 1) / 2.0;
        humidityValue = (humidityValue + 1) / 2.0;

        if (tempatureValue > 0.7)
        {
            if (humidityValue < 0.3)
                return "D"; // place holder for desert
            return "S"; // savanna placeholder
        }
        else if (tempatureValue > 0.4)
        {
            if (humidityValue > 0.5)
                return "F"; // forest placeholder
            return "P"; // plains placeholder
        }
        else
        {
            if (humidityValue > 0.6)
                return "T"; // taiga placeholder
            return "M"; // mountain placeholder
        }
    }

    static void DisplayWorld(string[][] world)
    {
        for (int y = 0; y < world.Length; y++)
        {
            for (int x = 0; x < world[y].Length; x++)
            {
                Console.Write(world[y][x]);
            }
            Console.WriteLine();
        }
    }
}