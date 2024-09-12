class DirtBlock : BaseBlock
{
    public DirtBlock() : base("Dirt", "dot:dirt")
    {
#if CLIENT
        Texture = BlockRegistry.GetBlock(BlockEnum.Dirt);
#endif
    }
}