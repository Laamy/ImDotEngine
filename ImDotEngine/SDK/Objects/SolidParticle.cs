using SFML.System;

class SolidParticle : SolidCircle
{
    public SolidParticle(float radius, uint points = 20) : base(radius, points) { }

    public Vector2f Velocity = new Vector2f();

    public override Vector2f GetSize() => new Vector2f(1, 1);
}