class StoneGrassyBlock : BaseBlock
{
    public StoneGrassyBlock() : base("Grassy Stone", "dot:grassy_stone")
    {
#if CLIENT
        Texture = BlockRegistry.GetBlock(BlockEnum.Grassy_Stone);
#endif
    }
}