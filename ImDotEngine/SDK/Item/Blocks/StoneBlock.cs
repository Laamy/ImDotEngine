class StoneBlock : BaseBlock
{
    public StoneBlock() : base("Stone", "dot:stone")
    {
#if CLIENT
        Texture = BlockRegistry.GetBlock(BlockEnum.Stone);
#endif
    }
}