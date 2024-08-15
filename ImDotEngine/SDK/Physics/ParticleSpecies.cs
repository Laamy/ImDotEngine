using SFML.Graphics;

class ParticleSpecies
{
    public Color Colour;
    public SpatialHash Particles; // list of atoms for each species

    public ParticleInfo info = new ParticleInfo();
}