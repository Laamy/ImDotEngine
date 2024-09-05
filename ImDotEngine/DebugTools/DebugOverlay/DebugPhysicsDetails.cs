using SFML.Graphics;
using SFML.System;
using System.Windows.Forms;

class DebugPhysicsDetails
{
    public static ClientInstance Instance = ClientInstance.GetSingle();

    public static void Draw(LocalPlayer entity, RenderWindow ctx)
    {
        // Draw the velocity vector & debug text for the speed
        {
            Vector2f lineDiff = new Vector2f(entity.curPos.X - entity.prevPos.X, entity.curPos.Y - entity.prevPos.Y);

            var lineStart = entity.curPos + new Vector2f(entity.Player.Size.X / 2, entity.Player.Size.Y);
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
                              + $"Pos:{entity.curPos}";
            speedDisplay.Position = lineStart + new Vector2f(20, lineDiff.Y / 3);

            ctx.Draw(speedDisplay.GetDrawable());
        }

        // Draw the prevPos
        {
            var prevPosBox = new RectangleShape(entity.Player.Size);

            prevPosBox.Position = entity.prevPos;
            prevPosBox.FillColor = Color.Transparent;
            prevPosBox.OutlineThickness = 3;
            prevPosBox.OutlineColor = Color.Blue;

            ctx.Draw(prevPosBox);
        }

        // Draw the curPos
        {
            var curPosBox = new RectangleShape(entity.Player.Size);

            curPosBox.Position = entity.curPos;
            curPosBox.FillColor = Color.Transparent;
            curPosBox.OutlineThickness = 2;
            curPosBox.OutlineColor = Color.Green;

            ctx.Draw(curPosBox);
        }
    }
}