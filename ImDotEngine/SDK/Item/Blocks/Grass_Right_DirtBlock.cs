class Grass_Right_DirtBlock : BaseBlock
{
    public Grass_Right_DirtBlock() : base("grass_dirt_right", "dot:grass_dirt_right")
    {
#if CLIENT
        Texture = BlockRegistry.GetBlock(BlockEnum.Grass_Right_Dirt);
#endif
    }
}