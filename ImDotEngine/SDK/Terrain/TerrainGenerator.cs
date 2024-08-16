using System;

class TerrainGenerator
{
    public static int Seed { get; set; } = new Random().Next(0, 1000000);

    public static BlockEnum GetBlock(BlockEnum[][] rawChunk, int x, int y)
    {
        if (y > rawChunk.Length || x > rawChunk.Length)
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

        for (int y = Y; y < height + Y; y++)
        {
            int chunkY = y - Y;

            rawChunk[y - Y] = new BlockEnum[width];
            for (int x = X; x < width + X; x++)
            {
                int chunkX = x - X;

                double noiseValue = perlin.GetValue((float)(x * 0.03), (float)(y * 0.03));
                int terrainHeight = (int)(5 + (noiseValue * 9));
                if (y > terrainHeight)
                {
                    // TODO: dont set until ready
                    rawChunk[chunkY][chunkX] = BlockEnum.Stone; // Terrain

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
            }
        }

        return rawChunk;
    }
}