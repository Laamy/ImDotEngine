class ChunkState
{
    public int chunkHash = -1;
    public BlockEnum[] blocks = new BlockEnum[16 * 16];// 16x16 for the blocks per chunk

    public ChunkState()
    {
        // initialize blockid array
        foreach (int i in blocks)
            blocks[i] = BlockEnum.Air;
    }
}