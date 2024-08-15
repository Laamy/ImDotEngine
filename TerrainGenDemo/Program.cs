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
        SimplexPerlin perlin = new SimplexPerlin(new Random().Next(0, 1000000));

        for (int y = 0; y < height; y++)
        {
            world[y] = new string[width];
            for (int x = 0; x < width; x++)
            {
                double noiseValue = perlin.GetValue((float)(x * 0.06), (float)(y * 0.06));
                int terrainHeight = (int)(5 + (noiseValue * 6)); // Adjust the parameters for terrain height
                if (y > terrainHeight)
                {
                    world[y][x] = "▀"; // Terrain
                }
                else
                {
                    world[y][x] = " "; // Empty space
                }
            }
        }

        return world;
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