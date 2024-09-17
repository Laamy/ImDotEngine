using SFML.System;

class ZoomComponent : IEntityComponent
{
    public float Zoom = 1;
    public float MaxZoom = 1;
    public float MinZoom = 0.1f;
}