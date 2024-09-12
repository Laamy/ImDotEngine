using SFML.Graphics;

using System.Collections.Generic;

#if CLIENT
class BlockRegistry
{
    private static ClientInstance Instance = ClientInstance.GetSingle();

    public static Dictionary<BlockEnum, string> blocks = new Dictionary<BlockEnum, string>()
    {
        { BlockEnum.Dirt, "Assets\\Texture\\dirt.png" },
        { BlockEnum.Stone, "Assets\\Texture\\stone.png" },

        { BlockEnum.Grass, "Assets\\Texture\\biome\\plain\\grass.png" }, // plain biome
        { BlockEnum.Grass_Right, "Assets\\Texture\\biome\\plain\\grass_right.png" },
        { BlockEnum.Grass_Right_Dirt, "Assets\\Texture\\biome\\plain\\grass_right_dirt.png" },
        { BlockEnum.Grass_Left, "Assets\\Texture\\biome\\plain\\grass_left.png" },
        { BlockEnum.Grass_Left_Dirt, "Assets\\Texture\\biome\\plain\\grass_left_dirt.png" },

        { BlockEnum.Sand, "Assets\\Texture\\biome\\desert\\sand.png" }, // desert biome

        { BlockEnum.Grassy_Stone, "Assets\\Texture\\biome\\cave\\grassy_stone.png" }, // cave biome
    };

    public static Texture GetBlock(BlockEnum block)
    {
        if (blocks.ContainsKey(block))
            return Instance.TextureRepository.GetTexture(blocks[block]);

        return null;
    }
}
#endif