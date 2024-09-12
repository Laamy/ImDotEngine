class Grass_Left_DirtBlock : BaseBlock
{
    public Grass_Left_DirtBlock() : base("grass_dirt_left", "dot:grass_dirt_left")
    {
#if CLIENT
        Texture = BlockRegistry.GetBlock(BlockEnum.Grass_Left_Dirt);
#endif
    }
}