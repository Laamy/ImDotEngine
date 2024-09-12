class GrassBlock : BaseBlock
{
    public GrassBlock() : base("Grass", "dot:grass")
    {
#if CLIENT
        Texture = BlockRegistry.GetBlock(BlockEnum.Grass);
#endif
    }
}