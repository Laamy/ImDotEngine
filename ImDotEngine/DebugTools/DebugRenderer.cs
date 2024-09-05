// unoptimized debug tools
using SFML.Graphics;
using SFML.System;

public class DebugRenderer
{
    // not used thickness cuz i just realized the lines type doesnt support that
    // NOTE: use rectangleshape with an angle/rotation for lines allowing for thickness
    public static void DrawLine(RenderWindow ctx, Vector2f start, Vector2f end, Color color, float thickness)
    {
        VertexArray line = new VertexArray(PrimitiveType.Lines, 2); // 2 points
        
        line[0] = new Vertex(start, color);
        line[1] = new Vertex(end, color);
        
        ctx.Draw(line);
    }
}