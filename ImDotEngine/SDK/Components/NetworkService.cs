using SFML.System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

#if CLIENT
class NetworkService : BaseService
{
    private ClientSocket socket;
    private ClientInstance Instance = ClientInstance.GetSingle();

    public NetworkService()
    {
        DebugLogger.Log("Components", $"Initialized : NetworkComponent");

        var tmc = Instance.Engine.Services.OfType<TerrainMorpherService>().FirstOrDefault();

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
            layer.Clear();

        {
            uint cellScale = 128;

            {
                for (int cX = 0; cX < 6; ++cX)
                {
                    for (int cY = 0; cY < 6; ++cY)
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
        var localPlayer = Instance.Engine.Services.OfType<LocalPlayerService>().FirstOrDefault();

        var playerupdate = ImPacket.Create<PlayerUpdatePacket>();
        
        var stateComp = localPlayer.Context.TryGetComponent<StateVectorComponent>();

        playerupdate.X = stateComp.CurPosition.X;
        playerupdate.Y = stateComp.CurPosition.Y;

        socket.Send(playerupdate.Encode());
    }

    public override void Initialized()
    {
        //socket = new ClientSocket("147.185.221.22", 12714);
        socket = new ClientSocket("127.0.0.1", 4746);

        socket.OnReceived += OnReceived;
    }

    private Dictionary<string, Tuple<Player, RigidBodyService>> players = new Dictionary<string, Tuple<Player, RigidBodyService>>();

    private async Task OnReceived(byte[] msg)
    {
        string message = Encoding.ASCII.GetString(msg);

        var packet = ImPacket.Decode(message);

        //DebugLogger.Log("NetworkComponent", $"{packet.Encode()}");// debug

        if (packet is HandshakePacket handshake)
        {
            // TEMP CODE
            if (handshake.AllowChunkGen)
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

            RigidBodyService body = new RigidBodyService();
            {
                var stateComp = body.Context.TryGetComponent<StateVectorComponent>();

                body.BodyRoot = new SolidObject();

                body.ActiveCamera = false; // disable camera on this body

                body.BodyRoot.Position = new Vector2f(100, 0);
                body.BodyRoot.Size = new Vector2f(38, 65);
                body.Context.EmplaceComponent<FlagComponent<AnchorFlag>>();
                //BodyRoot.Color = Color.Red;

                var playerAsset = Instance.TextureRepository.GetTexture("Assets\\Texture\\player\\female.png");

                body.BodyRoot.Texture = playerAsset;

                stateComp.PrevPosition = body.BodyRoot.Position;
                stateComp.CurPosition = body.BodyRoot.Position;
            }

            // add player to list
            players.Add(playeradd.UUID, new Tuple<Player, RigidBodyService>(player, body));

            // add to scene as an actual physics object
            Instance.Engine.Services.Add(body);
        }

        // remove disconnected players cuz their USELESS !
        if (packet is PlayerRemovePacket playerremove)
        {
            if (!players.ContainsKey(playerremove.UUID))
                return;

            var player = players[playerremove.UUID];

            players.Remove(player.Item1.UUID);
            Instance.Engine.Services.Remove(player.Item2);
        }

        // NOTE: sometimes the players aren't smoothed out between packets
        if (packet is PlayerUpdatePacket playerupdate)
        {
            if (!players.ContainsKey(playerupdate.UUID))
                return;

            // game should smooth it out
            var bodycomp = players[playerupdate.UUID].Item2;

            var stateComp = bodycomp.Context.TryGetComponent<StateVectorComponent>();

            stateComp.PrevPosition = stateComp.CurPosition; // bruh
            stateComp.CurPosition = new Vector2f(playerupdate.X, playerupdate.Y);
            stateComp.Velocity = new Vector2f(playerupdate.VX, playerupdate.VY);
        }

        if (packet is PlayerBouncePacket playerbounce)
        {
            // reset player info to this (we wont be smoothing this out..)

            var localPlayer = Instance.Engine.Services.OfType<LocalPlayerService>().FirstOrDefault();

            var stateComp = localPlayer.Context.TryGetComponent<StateVectorComponent>();

            stateComp.CurPosition = new Vector2f(playerbounce.X, playerbounce.Y);
            stateComp.PrevPosition = stateComp.CurPosition;
            localPlayer.BodyRoot.Position = stateComp.CurPosition;

            stateComp.Velocity = new Vector2f(playerbounce.VX, playerbounce.VY);
        }
    }
}
#endif