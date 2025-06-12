using System;
using System.Collections.Generic;

class OreRegistry
{
    public class OreEntry
    {
        public BlockEnum oreBlock = BlockEnum.Stone_Iron;
        public int oreRarity = 13;
        public int minSize = 1,
            maxSize = 9;

        public OreEntry(BlockEnum oreBlock, int oreRarity, int minSize, int maxSize)
        {
            this.oreBlock = oreBlock;
            this.oreRarity = oreRarity;
            this.minSize = minSize;
            this.maxSize = maxSize;
        }
    }

    public static Dictionary<BlockEnum, OreEntry> Ores { get; private set; } = new Dictionary<BlockEnum, OreEntry>()
    {
        { BlockEnum.Stone_Iron, new OreEntry(BlockEnum.Stone_Iron, 13, 1, 9) },
        { BlockEnum.Dirt, new OreEntry(BlockEnum.Dirt, 2, 75,250) },
    };
}

class TerrainGenerator
{
    public static int Seed { get; set; } = new Random().Next(0, 1000000);

    public static BlockEnum GetBlock(BlockEnum[][] rawChunk, int x, int y)
    {
        if (y > rawChunk.Length - 1 || x > rawChunk.Length - 1)
            return BlockEnum.None;

        if (y < 0 || x < 0)
            return BlockEnum.None;

        return rawChunk[y][x];
    }

    public static BlockEnum[][] GenerateChunk(int X, int Y)
    {
        int width = 16, 
            height = 16;

        BlockEnum[][] rawChunk = new BlockEnum[height][];
        SimplexPerlin perlin = new SimplexPerlin(Seed);

        // for biomes
        SimplexPerlin temperaturePerlin = new SimplexPerlin(Seed);
        SimplexPerlin humidityPerlin = new SimplexPerlin(Seed);

        for (int y = Y; y < height + Y; y++)
        {
            int chunkY = y - Y;

            rawChunk[y - Y] = new BlockEnum[width];
            for (int x = X; x < width + X; x++)
            {
                int chunkX = x - X;

                double tempatureValue = temperaturePerlin.GetValue((float)(x * 0.001), (float)(y * 0.001));
                double humidityValue = humidityPerlin.GetValue((float)(x * 0.001), (float)(y * 0.001));

                BiomeEnum biome = DetermineBiome(tempatureValue, humidityValue);

                double noiseValue = perlin.GetValue((float)(x * 0.03), (float)(y * 0.03));
                int terrainHeight = (int)(5 + (noiseValue * 9));
                if (y > terrainHeight)
                {
                    // NOTE: dont set until ready
                    rawChunk[chunkY][chunkX] = BlockEnum.Stone; // Terrain

                    if (biome == BiomeEnum.Plains)
                    {
                        // grass check
                        if (GetBlock(rawChunk, chunkX, chunkY - 1) == BlockEnum.Air)
                            rawChunk[chunkY][chunkX] = BlockEnum.Grass;

                        // dirt check, TODO: clean generation code
                        if (
                            GetBlock(rawChunk, chunkX, chunkY - 1) == BlockEnum.Grass ||
                            GetBlock(rawChunk, chunkX, chunkY - 2) == BlockEnum.Grass ||
                            GetBlock(rawChunk, chunkX, chunkY - 3) == BlockEnum.Grass
                        )
                            rawChunk[chunkY][chunkX] = BlockEnum.Dirt;
                    }

                    if (biome == BiomeEnum.Mountain)
                    {
                        if (GetBlock(rawChunk, chunkX, chunkY - 1) == BlockEnum.Air)
                        {
                            rawChunk[chunkY-1][chunkX] = BlockEnum.Stone;
                        }
                    }

                    if (biome == BiomeEnum.Forest)
                    {
                        // grass check
                        if (GetBlock(rawChunk, chunkX, chunkY - 1) == BlockEnum.Air)
                            rawChunk[chunkY][chunkX] = BlockEnum.Grass;

                        // dirt check, TODO: clean generation code
                        if (
                            GetBlock(rawChunk, chunkX, chunkY - 1) == BlockEnum.Grass ||
                            GetBlock(rawChunk, chunkX, chunkY - 2) == BlockEnum.Grass ||
                            GetBlock(rawChunk, chunkX, chunkY - 3) == BlockEnum.Grass
                        )
                            rawChunk[chunkY][chunkX] = BlockEnum.Dirt;
                    }

                    if (biome == BiomeEnum.Desert)
                    {
                        // dirt check, TODO: clean generation code
                        if (
                            GetBlock(rawChunk, chunkX, chunkY) == BlockEnum.Stone
                        )
                            rawChunk[chunkY][chunkX] = BlockEnum.Sand;
                    }
                }
                else
                {
                    rawChunk[chunkY][chunkX] = BlockEnum.Air; // Empty space
                }
            }
        }

        int caveThresholdY = 20;

        // caves
        {
            float baseCaveWeight = 0.3f;
            int minCaveSize = 20;

            bool[,] visited = new bool[height, width];

            int FloodFill(int startX, int startY)
            {
                int size = 0;
                Queue<(int x, int y)> q = new Queue<(int x, int y)>();
                q.Enqueue((startX, startY));
                visited[startY, startX] = true;
                int[] dx = { 1, -1, 0, 0 };
                int[] dy = { 0, 0, 1, -1 };

                while (q.Count > 0)
                {
                    var (x, y) = q.Dequeue();
                    size++;

                    for (int i = 0; i < 4; i++)
                    {
                        int nx = x + dx[i];
                        int ny = y + dy[i];

                        if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                        {
                            if (!visited[ny, nx] && rawChunk[ny][nx] == BlockEnum.Air)
                            {
                                visited[ny, nx] = true;
                                q.Enqueue((nx, ny));
                            }
                        }
                    }
                }

                return size;
            }

            for (int y = Y; y < height + Y; y++)
            {
                int chunkY = y - Y;

                if (y >= caveThresholdY)
                {
                    for (int x = X; x < width + X; x++)
                    {
                        int chunkX = x - X;

                        double wormNoiseX = perlin.GetValue((float)(x * baseCaveWeight) / 20, (float)(y * baseCaveWeight) / 10);
                        double wormNoiseY = perlin.GetValue((float)(x * baseCaveWeight), (float)(y * baseCaveWeight));

                        if (wormNoiseX > 0.3f || wormNoiseY > 0.6f)
                        {
                            rawChunk[chunkY][chunkX] = BlockEnum.Air;
                        }
                    }
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!visited[y, x] && rawChunk[y][x] == BlockEnum.Air)
                    {
                        int caveSize = FloodFill(x, y);
                        if (caveSize < minCaveSize)
                        {
                            RemoveCave(x, y);
                        }
                    }
                }
            }

            void RemoveCave(int startX, int startY)
            {
                Queue<(int x, int y)> q = new Queue<(int x, int y)>();
                q.Enqueue((startX, startY));
                visited[startY, startX] = true;

                int[] dx = { 1, -1, 0, 0 };
                int[] dy = { 0, 0, 1, -1 };

                while (q.Count > 0)
                {
                    var (x, y) = q.Dequeue();
                    rawChunk[y][x] = BlockEnum.Stone;

                    for (int i = 0; i < 4; i++)
                    {
                        int nx = x + dx[i];
                        int ny = y + dy[i];

                        if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                        {
                            if (rawChunk[ny][nx] == BlockEnum.Air)
                            {
                                rawChunk[ny][nx] = BlockEnum.Stone;
                                q.Enqueue((nx, ny));
                            }
                        }
                    }
                }
            }
        }

        // ores
        Random oreRng = new Random(Seed + 1 + X * 73856093 + Y * 19349663);
        foreach (var ore in OreRegistry.Ores)
        {
            int oreRarity = ore.Value.oreRarity;

            for (int y = Y; y < height + Y; y++)
            {
                int chunkY = y - Y;

                if (y >= caveThresholdY)
                {
                    for (int x = X; x < width + X; x++)
                    {
                        int chunkX = x - X;

                        if (oreRng.Next(0, 1000) < oreRarity)
                        {
                            int oreSize = oreRng.Next(ore.Value.minSize, ore.Value.maxSize);
                            int dx = x;
                            int dy = y;
                            int dirX = oreRng.Next(-1, 2);
                            int dirY = oreRng.Next(-1, 2);

                            for (int i = 0; i < oreSize; i++)
                            {
                                int cx = dx - X;
                                int cy = dy - Y;

                                if (cx >= 0 && cx < width && cy >= 0 && cy < height)
                                {
                                    if (rawChunk[cy][cx] == BlockEnum.Stone)
                                        rawChunk[cy][cx] = ore.Value.oreBlock;
                                }

                                if (oreRng.Next(0, 2) == 0)
                                {
                                    int topY = dy - 1 - Y;
                                    if (cx >= 0 && cx < width && topY >= 0 && topY < height)
                                    {
                                        if (rawChunk[topY][cx] == BlockEnum.Stone)
                                            rawChunk[topY][cx] = ore.Value.oreBlock;
                                    }
                                }

                                if (oreRng.Next(0, 4) == 0)
                                {
                                    dirX = oreRng.Next(-1, 2);
                                    dirY = oreRng.Next(-1, 2);
                                }

                                dx += dirX;
                                dy += dirY;
                            }
                        }
                    }
                }
            }

        }

        // experiments
        for (int y = Y; y < height + Y; y++)
        {
            int chunkY = y - Y;

            for (int x = X; x < width + X; x++)
            {
                int chunkX = x - X;

                var block = GetBlock(rawChunk, chunkX, chunkY);
        
                if (block == BlockEnum.Stone &&
                    GetBlock(rawChunk, chunkX, chunkY - 1) == BlockEnum.Air &&
                    GetBlock(rawChunk, chunkX, chunkY - 2) == BlockEnum.Air
                )
                {
                    rawChunk[chunkY][chunkX] = BlockEnum.Grassy_Stone;
                }

                // experimental grass corners
                //BUG: need to sort out chunk boundaries not connecting properly
                //BUG: if the block to the tops side is dirt aswell then it means its still an angle and needs the grass sides
                {
                    if (block == BlockEnum.Grass &&
                    GetBlock(rawChunk, chunkX - 1, chunkY - 1) == BlockEnum.Grass)
                    {
                        rawChunk[chunkY - 1][chunkX] = BlockEnum.Grass_Right;
                        rawChunk[chunkY][chunkX] = BlockEnum.Grass_Right_Dirt;
                    }

                    if (block == BlockEnum.Grass &&
                        GetBlock(rawChunk, chunkX + 1, chunkY - 1) == BlockEnum.Grass)
                    {
                        rawChunk[chunkY - 1][chunkX] = BlockEnum.Grass_Left;
                        rawChunk[chunkY][chunkX] = BlockEnum.Grass_Left_Dirt;
                    }
                }
            }
        }

        return rawChunk;
    }

    private static BiomeEnum DetermineBiome(double tempatureValue, double humidityValue)
    {
        tempatureValue = (tempatureValue + 1) / 2.0;
        humidityValue = (humidityValue + 1) / 2.0;

        if (tempatureValue > 0.7)
        {
            //if (humidityValue < 0.3)
               return BiomeEnum.Desert;
            //return BiomeEnum.Savanna;
        }
        else if (tempatureValue > 0.4)
        {
            if (humidityValue > 0.5)
                return BiomeEnum.Forest;
            return BiomeEnum.Plains;
        }
        else
        {
            //if (humidityValue > 0.6)
            //    return BiomeEnum.Taiga;
            return BiomeEnum.Mountain;
        }
    }
}