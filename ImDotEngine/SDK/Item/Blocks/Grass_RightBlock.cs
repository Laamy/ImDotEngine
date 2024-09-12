class Grass_RightBlock : BaseBlock
{
    public Grass_RightBlock() : base("grass_right", "dot:grass_right")
    {
#if CLIENT
        Texture = BlockRegistry.GetBlock(BlockEnum.Grass_Right);
#endif
    }
}