# ImDotEngine

Super basic engine (not for production use) that I can use to fuck around and learn new things

# Showcase

you can see a basic showcase of the sample scene & engine
[here](https://streamable.com/npwgbz)

# Features

1. Spatial Partitioning using hashes so you dont have to loop over every object when rendering or doing physic steps
2. Exclusive/Borderless fullscreen (No optimized fullscreen support at this point in time)
3. Easy to extend on the game with components that take in all the default game events as overrides
4. Layers for the level so you can have layers for UI foreground and background (or more!)

# Planned

1. A particle-life simulation as the sample scene that supports up to 100k particles
2. group objects together as a texture for static scenes to reduce rendering costs
3. batched rendering

# Bug fixes (TODO)

SolidConvex:57 - check largest/farthest vertex and use as size for both X & Y
