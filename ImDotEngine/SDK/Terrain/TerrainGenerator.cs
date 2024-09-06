using System;

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
        int width = 24, 
            height = 24;

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

        float baseCaveWeight = 0.1f;
        int caveThresholdY = 20;
        
        for (int y = Y; y < height + Y; y++)
        {
            int chunkY = y - Y;
        
            if (y >= caveThresholdY)
            {
                for (int x = X; x < width + X; x++)
                {
                    int chunkX = x - X;
        
                    double wormNoiseX = perlin.GetValue((float)(x * baseCaveWeight) / 20, (float)(y * baseCaveWeight));
                    double wormNoiseY = perlin.GetValue((float)(x * baseCaveWeight), (float)(y * baseCaveWeight));
        
                    if (wormNoiseX > 0.5f || wormNoiseY > 0.6f)
                    {
                        rawChunk[chunkY][chunkX] = BlockEnum.Air;
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