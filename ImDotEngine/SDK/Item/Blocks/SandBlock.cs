class SandBlock : BaseBlock
{
    public SandBlock() : base("Sand", "dot:sand")
    {
#if CLIENT
        Texture = BlockRegistry.GetBlock(BlockEnum.Sand);
#endif
    }
}