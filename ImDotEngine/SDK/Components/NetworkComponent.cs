using SFML.System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if CLIENT
class NetworkComponent : BaseComponent
{
    private ClientSocket socket;
    private ClientInstance Instance = ClientInstance.GetSingle();

    public NetworkComponent()
    {
        DebugLogger.Log("Components", $"Initialized : NetworkComponent");

        var tmc = Instance.Engine.Components.OfType<TerrainMorpherComponent>().FirstOrDefault();

        tmc.OnChunkChanged += OnChunkChanged;
    }

    // what the fuck is this mess i just wrote
    // NOTE: rewrite this
    void OnChunkChanged(int chunkHash)
    {
        // initialize a new packet & chunkstate for our changes
        var packet = ImPacket.Create<WorldStatePacket>();

        ChunkState chunkState = new ChunkState()
        {
            chunkHash = chunkHash,
        };

        // NOTE: might cause some bugs in the future (though 2 chunks should never be in the same region..)
        // get the chunk by the hash provided
        var chunkByHash = Instance.Level.GetLayer(LevelLayers.ForeBlocks).GetRegionByHash(chunkHash);

        // convert chunk to SolidGroup
        var chunk = chunkByHash.FirstOrDefault() as SolidGroup;

        // copy all the ID's from the chunk to the chunkstate
        int blockId = 0;
        foreach (var _block in chunk.GetObjects())
        {
            var block = _block as SolidObject;

            chunkState.blocks[blockId] = (BlockEnum)block.Tags[0];
            blockId++;
        }

        // finally add the chunk to the packet & send it
        packet.states.Add(chunkState);
        
        socket.Send(packet.Encode());
    }

    // temp messy fucked code
    // TODO: reuse rectangleshape objects to lower memory usage (some kind of hash)
    public void GenerateWorld(int seed)
    {
        TerrainGenerator.Seed = seed;

        var layer = Instance.Level.GetLayer(LevelLayers.ForeBlocks);

        if (layer.Count > 0)
        {
            // TODO: add a way to clear a spatial partioning hashmap completely
        }

        {
            uint cellScale = 128;

            {
                for (int cX = 0; cX < 10; ++cX)
                {
                    for (int cY = 0; cY < 2; ++cY)
                    {
                        // texture atlas/object group (not scaled up or down cuz its a fucking square)
                        SolidGroup group = new SolidGroup(new TextureAtlas((uint)(16 * cellScale), (uint)(16 * cellScale)));

                        group.Position = new Vector2f(cX * (cellScale * (cellScale / 30)), cY * (cellScale * (cellScale / 30)));

                        var chunk = TerrainGenerator.GenerateChunk(cX * 16, cY * 16);

                        for (int y = 0; y < 16; ++y)
                        {
                            for (int x = 0; x < 16; ++x)
                            {
                                var block = chunk[y][x];

                                if (block == BlockEnum.Air)
                                    continue;

                                //var shader = Instance.MaterialRepository.GetShader($"Shaders\\texture_noise.frag");

                                SolidObject chunkBlock = new SolidObject();

                                chunkBlock.Tags.Add(block);
                                chunkBlock.Position = new Vector2f(x * cellScale, y * cellScale);
                                chunkBlock.Size = new Vector2f(cellScale, cellScale);

                                var blockResource = BlockRegistry.GetBlock(block);

                                if (blockResource != null) // valid block
                                    chunkBlock.Texture = BlockRegistry.GetBlock(block);

                                group.AddObject(chunkBlock);
                            }
                        }

                        group.Scale = new Vector2f(0.25f, 0.25f);
                        group.Invalidate(); // refresh texture atlas

                        layer.AddObject(group);
                    }
                }
            }
        }

        DebugLogger.Log("NetworkComponent", "Terrain generation finished");
    }

    public override void OnFixedUpdate()
    {
        var localPlayer = Instance.Engine.Components.OfType<LocalPlayer>().FirstOrDefault();

        var playerupdate = ImPacket.Create<PlayerUpdatePacket>();

        playerupdate.X = localPlayer.curPos.X;
        playerupdate.Y = localPlayer.curPos.Y;

        socket.Send(playerupdate.Encode());
    }

    public override void Initialized()
    {
        //socket = new ClientSocket("147.185.221.22", 12714);
        socket = new ClientSocket("127.0.0.1", 4746);

        socket.OnReceived += OnReceived;
    }

    private Dictionary<string, Tuple<Player, RigidBodyComponent>> players = new Dictionary<string, Tuple<Player, RigidBodyComponent>>();

    private async Task OnReceived(byte[] msg)
    {
        string message = Encoding.ASCII.GetString(msg);

        var packet = ImPacket.Decode(message);

        //DebugLogger.Log("NetworkComponent", $"{packet.Encode()}");// debug

        if (packet is HandshakePacket handshake)
        {
            // generate servers terrain on the client
            GenerateWorld(handshake.WorldSeed);

            Instance.AllowPhysics = true; // world is finished generating so place player in
        }

        // add player when player connects
        if (packet is PlayerAddPacket playeradd)
        {
            if (players.ContainsKey(playeradd.UUID))
                return;

            var player = new Player()
            {
                UUID = playeradd.UUID,
                X = playeradd.X,
                Y = playeradd.Y
            };

            RigidBodyComponent body = new RigidBodyComponent();
            {
                body.BodyRoot = new SolidObject();

                body.ActiveCamera = false; // disable camera on this body

                body.BodyRoot.Position = new Vector2f(100, 0);
                body.BodyRoot.Size = new Vector2f(38, 65);
                body.IsAnchored = true; // temp
                //BodyRoot.Color = Color.Red;

                var playerAsset = Instance.TextureRepository.GetTexture("Assets\\Texture\\player\\female.png");

                body.BodyRoot.Texture = playerAsset;

                body.prevPos = body.BodyRoot.Position;
                body.curPos = body.BodyRoot.Position;
            }

            // add player to list
            players.Add(playeradd.UUID, new Tuple<Player, RigidBodyComponent>(player, body));

            // add to scene as an actual physics object
            Instance.Engine.Components.Add(body);
        }

        // remove disconnected players cuz their USELESS !
        if (packet is PlayerRemovePacket playerremove)
        {
            if (!players.ContainsKey(playerremove.UUID))
                return;

            var player = players[playerremove.UUID];

            players.Remove(player.Item1.UUID);
            Instance.Engine.Components.Remove(player.Item2);
        }

        // NOTE: sometimes the players aren't smoothed out between packets
        if (packet is PlayerUpdatePacket playerupdate)
        {
            if (!players.ContainsKey(playerupdate.UUID))
                return;

            // game should smooth it out
            var bodycomp = players[playerupdate.UUID].Item2;

            bodycomp.prevPos = bodycomp.curPos; // bruh
            bodycomp.curPos = new Vector2f(playerupdate.X, playerupdate.Y);
            bodycomp.Velocity = new Vector2f(playerupdate.VX, playerupdate.VY);
        }

        if (packet is PlayerBouncePacket playerbounce)
        {
            // reset player info to this (we wont be smoothing this out..)

            var localPlayer = Instance.Engine.Components.OfType<LocalPlayer>().FirstOrDefault();

            localPlayer.curPos = new Vector2f(playerbounce.X, playerbounce.Y);
            localPlayer.prevPos = localPlayer.curPos;
            localPlayer.BodyRoot.Position = localPlayer.curPos;

            localPlayer.Velocity = new Vector2f(playerbounce.VX, playerbounce.VY);
        }
    }
}
#endif