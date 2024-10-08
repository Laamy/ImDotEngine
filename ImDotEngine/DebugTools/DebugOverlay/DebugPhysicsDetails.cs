﻿using SFML.Graphics;
using SFML.System;

#if CLIENT
class DebugPhysicsDetails
{
    public static ClientInstance Instance = ClientInstance.GetSingle();

    public static void Draw(RigidBodyComponent entityComponent, RenderWindow ctx)
    {
        // gonna just reuse this
        RectangleShape debugBox = new RectangleShape(new Vector2f(0, 0));
        debugBox.OutlineThickness = 2;
        debugBox.OutlineColor = new Color(0, 255, 0, 255);

        var nearbyChunks = entityComponent.GetNearby();

        // draw the chunks bounding box
        {
            foreach (var _chunk in nearbyChunks)
            {
                if (_chunk != null)
                {
                    // its just accured to me that i scale the group
                    var chunk = _chunk as SolidGroup;

                    debugBox.FillColor = new Color(0, 0, 255, 255 / 5);
                    debugBox.OutlineThickness = 2;
                    debugBox.Size = chunk.GetSize();
                    debugBox.Position = chunk.GetPosition();

                    ctx.Draw(debugBox);
                }
            }
        }

        debugBox.FillColor = new Color(255, 0, 0, 255 / 5);
        debugBox.Size = new Vector2f(0, 0);

        // push nearby collidables as red partially transparent squares
        {
            foreach (var _chunk in nearbyChunks)
            {
                if (_chunk != null)
                {
                    // its just accured to me that i scale the group
                    var chunk = _chunk as SolidGroup;

                    var blocks = chunk.GetObjects();

                    foreach (var block in blocks)
                    {
                        // convert blockRect to worldspace
                        var blockRect = new FloatRect(chunk.GetPosition() + block.GetPosition().Mul(chunk.Scale), block.GetSize().Mul(chunk.Scale));

                        if (debugBox.Size.X == 0)
                            debugBox.Size = new Vector2f(blockRect.Width, blockRect.Height);

                        debugBox.Position = new Vector2f(blockRect.Left, blockRect.Top);

                        ctx.Draw(debugBox);
                    }
                }
            }
        }

        // Draw the velocity vector & debug text for the speed
        {
            Vector2f lineDiff = new Vector2f(entityComponent.curPos.X - entityComponent.prevPos.X, entityComponent.curPos.Y - entityComponent.prevPos.Y);

            var lineStart = entityComponent.curPos + new Vector2f(entityComponent.BodyRoot.Size.X / 2, entityComponent.BodyRoot.Size.Y);
            var lineEnd = lineStart + lineDiff;

            DebugRenderer.DrawLine(ctx, lineStart, lineEnd, Color.Red, 1);

            SolidText speedDisplay = new SolidText();

            speedDisplay.Font = Instance.FontRepository.GetFont("Arial");
            speedDisplay.Size = 16;
            speedDisplay.Color = Color.Red;
            speedDisplay.Text = $"XY:{lineDiff.Magnitude()}u/s\r\n"
                              + $" X:{Mathf.Abs(lineDiff.X)}u/s\r\n"
                              + $" Y:{Mathf.Abs(lineDiff.Y)}u/s\r\n"
                              + $"\r\n"
                              + $"Pos:{entityComponent.curPos}\r\n"
                              + $"OnGround: {entityComponent.Context.HasComponent<FlagComponent<OnGroundFlag>>()}";
            speedDisplay.Position = lineStart + new Vector2f(20, lineDiff.Y);

            ctx.Draw(speedDisplay.GetDrawable());
        }

        // Draw the prevPos
        {
            var prevPosBox = new RectangleShape(entityComponent.BodyRoot.Size);

            prevPosBox.Position = entityComponent.prevPos;
            prevPosBox.FillColor = Color.Transparent;
            prevPosBox.OutlineThickness = 3;
            prevPosBox.OutlineColor = Color.Blue;

            ctx.Draw(prevPosBox);
        }

        // Draw the curPos
        {
            var curPosBox = new RectangleShape(entityComponent.BodyRoot.Size);

            curPosBox.Position = entityComponent.curPos;
            curPosBox.FillColor = Color.Transparent;
            curPosBox.OutlineThickness = 2;
            curPosBox.OutlineColor = Color.Green;

            ctx.Draw(curPosBox);
        }
    }
}
#endif