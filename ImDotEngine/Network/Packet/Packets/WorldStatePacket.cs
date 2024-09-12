using System;
using System.Collections.Generic;
using System.Text;

class ChunkState
{
    public int chunkHash = -1;
    public BlockEnum[] blocks = new BlockEnum[16*16];// 16x16 for the blocks per chunk

    public ChunkState()
    {
        // initialize blockid array
        foreach (int i in blocks)
            blocks[i] = BlockEnum.Air;
    }
}

// TODO: find a way to get larger then 16x16 chunks for LINUX (they cap at 2048x2048 for the texture atlas which is 16x16 blocks)
class WorldStatePacket : Packet
{
    // unoptimized for now
    public List<ChunkState> states = new List<ChunkState>();

    public WorldStatePacket() : base(PacketType.WORLD_STATE) { }
    public WorldStatePacket(string format) : base(PacketType.WORLD_STATE) => Decode(format);

    // this should work
    public override void Decode(string format)
    {
        Console.WriteLine(format);
        var parts = format.Split(':');

        if (parts[0] != Name)
            throw new Exception("Attempt to unpack invalid packet");

        {
            var variables = parts[1].Trim().Split(',');

            var chunks = variables[0].Split(' ');

            // split states list variable into each seperate state
            foreach (var chunk in chunks)
            {
                var state = new ChunkState();

                var spots = chunk.Split('.');

                // unload block id's into chunkstate
                for (int i = 0; i < spots.Length - 1; ++i) // im to lazy to remove the leading dot.. (NOTE: add the .Trim(new char[] { '.' }))
                    state.blocks[i] = (BlockEnum)Convert.ToInt32(spots[i]);

                states.Add(state);
            }
        }
    }

    public override string Encode()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($"{Name}:");

        foreach (var chunk in states)
        {
            stringBuilder.Append(chunk.chunkHash); // position hashed

            foreach (var block in chunk.blocks)
                stringBuilder.Append($"{(int)block}.");

            stringBuilder.Append(' '); // end of chunk
        }

        return stringBuilder.ToString().Trim(); // remove leading space
    }
}