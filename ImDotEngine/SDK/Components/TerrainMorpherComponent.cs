using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Linq;

#if CLIENT
class TerrainMorpherComponent : BaseComponent
{
    public ClientInstance Instance = ClientInstance.GetSingle();

    // event for when block place or break
    public event Action<int> OnChunkChanged; 

    // TODO: proper worldstate packets to and from server
    public override void MouseButtonPressed(MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Left)
        {
            Camera2D camera = Instance.Engine.Components.OfType<Camera2D>().FirstOrDefault();

            Vector2f inWorld = camera.CursorToWorld(Instance.Engine.window, new Vector2f(e.X, e.Y));

            var nearbyChunks = Instance.Level.GetLayer(LevelLayers.ForeBlocks).GetNearbyObjects(inWorld, 1);

            foreach (var _chunk in nearbyChunks)
            {
                var chunk = _chunk as SolidGroup;

                var tiles = chunk.GetObjects();

                foreach (var block in tiles)
                {
                    if (block != null)
                    {
                        // convert local coords to global & create rect
                        // TODO: make a util for this (inside of the solidgroup class)
                        var blockRect = new FloatRect(
                            chunk.GetPosition() + block.GetPosition().Mul(chunk.Scale),
                            block.GetSize().Mul(chunk.Scale)
                        );

                        if (blockRect.Intersects(new FloatRect(inWorld, new Vector2f(1, 1))))
                        {
                            chunk.RemoveObject(block);
                            chunk.Invalidate(); // forgot to redraw

                            OnChunkChanged?.Invoke(Instance.Level.HashPosition(chunk.GetPosition()));
                        }
                    }
                }
            }

            DebugLogger.Warn($"clicked {inWorld}");
        }
    }

    public TerrainMorpherComponent()
    {
        DebugLogger.Log("Components", $"Initialized : TerrainMorpher");
    }
}
#endif