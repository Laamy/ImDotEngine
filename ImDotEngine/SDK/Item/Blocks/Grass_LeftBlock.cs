class Grass_LeftBlock : BaseBlock
{
    public Grass_LeftBlock() : base("grass_left", "dot:grass_left")
    {
#if CLIENT
        Texture = BlockRegistry.GetBlock(BlockEnum.Grass_Left);
#endif
    }
}