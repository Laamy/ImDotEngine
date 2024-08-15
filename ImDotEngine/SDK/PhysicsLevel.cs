using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

// MIGHT USE THIS FOR A PERMA RULESET!
// 0:0 0.524,0:1 -0.432,0:2 0.596,1:1 -0.616,1:2 0.632,2:2 0.18;0:0 0.252,0:1 -0.656,0:2 -0.788,1:1 0.844,1:2 0.524,2:2 0.108;0:0 -0.244,0:1 -0.184,0:2 -0.272,1:1 -0.248,1:2 -0.448,2:2 -0.076;0:0 -0.232,0:1 0.684,0:2 0.664,1:1 -0.668,1:2 0.396,2:2 0.016;0:0 0.504,0:1 0.248,0:2 -0.808,1:1 -0.74,1:2 -0.58,2:2 -0.796;0:0 0.264,0:1 -0.588,0:2 -0.688,1:1 0.848,1:2 0.54,2:2 0.172;0:0 0.724,0:1 0.544,0:2 -0.288,1:1 0.716,1:2 0.608,2:2 -0.536;0:0 0.888,0:1 -0.012,0:2 -0.032,1:1 0.584,1:2 -0.416,2:2 -0.888
class PhysicsLevel
{
    private Random ran = new Random();

    public const int WorldSize = 500;
    public const float SpeedOfLight = 9000000;

    // TODO make hashset once we start adding large amount of species for performance
    public List<ParticleSpecies> ParticleSpecies = new List<ParticleSpecies>(); // list of species
    public Dictionary<(int, int), float> interactMatrices = new Dictionary<(int, int), float>(); // interaction matrices

    // unused!
    public void Rule(Vector2f position, ParticleInfo info)
    {
        Parallel.ForEach(ClientInstance.GetSingle().Level.GetLayer(LevelLayers.Foreground).GetObjectsInBounds(
            new FloatRect(new Vector2f(0, 0), new Vector2f(500, 500))), actor =>
        {
            SolidParticle atom = actor as SolidParticle;

            //if (atom == null) // wont be null but just incase yk lil sis?
            //    return;

            float dx = position.X - atom.Position.X;
            float dy = position.Y - atom.Position.Y;

            int d = (int)Math.Sqrt(dx * dx + dy * dy);

            if (d <= 80)
            {
                float force = 10 * 1 / d;

                atom.Velocity.X += force * dx;
                atom.Velocity.Y += force * dy;
            }
        });
    }

    // my version of this video https://www.youtube.com/watch?v=0Kx4Y9TVMGg
    public void Rule(ParticleSpecies species1, ParticleSpecies species2, float g)
    {
        //for (int i = 0; i < species1.Particles.Length; ++i)
        Parallel.ForEach(species1.Particles.GetObjectsInBounds(new FloatRect(0, 0, WorldSize, WorldSize)), obj =>
        {
            SolidParticle p1 = obj as SolidParticle;

            float fx = 0;
            float fy = 0;

            foreach (var p2 in species2.Particles.GetNearbyObjects(p1.Position, species2.info.GravityStrength))
            {
                float dx = p1.Position.X - p2.Position.X;
                float dy = p1.Position.Y - p2.Position.Y;

                float d = (float)Math.Sqrt(dx * dx + dy * dy);
                if (d > 0 && d < species2.info.GravityStrength) // 80^2
                {
                    float F = g * 1 / d;
                    fx += F * dx;
                    fy += F * dy;
                }
            }

            fx *= species1.info.RuleDamping;
            fy *= species1.info.RuleDamping;

            // velocity
            p1.Velocity.X = (p1.Velocity.X + fx) * species1.info.SlowMultiplier;
            p1.Velocity.Y = (p1.Velocity.Y + fy) * species1.info.SlowMultiplier;

            UpdateParticleColor(p1);

            // update based on velocity
            p1.Position += p1.Velocity;

            species1.Particles.UpdateObject(p1);

            //p1.Position.X += p1.Velocity.X;
            //p1.Position.Y += p1.Velocity.Y;

            CheckBounds(p1);
        });
    }

    private void UpdateParticleColor(SolidParticle p1)
    {
        float maxVelocity = 3f;
        float minVelocity = -3f;

        float normalizedVelocityX = (p1.Velocity.X - minVelocity) / (maxVelocity - minVelocity);
        float normalizedVelocityY = (p1.Velocity.Y - minVelocity) / (maxVelocity - minVelocity);

        float combinedVelocity = (normalizedVelocityX + normalizedVelocityY) / 2f;

        int alpha = (int)(combinedVelocity * 240) + 15;

        if (alpha < 15) alpha = 15;
        if (alpha > 255) alpha = 255;

        Color colorWithAlpha = new Color(p1.Color.R, p1.Color.G, p1.Color.B, (byte)alpha);

        p1.Color = colorWithAlpha;
    }

    private void CheckBounds(SolidParticle p1)
    {
        if (p1.Position.X >= WorldSize)
        {
            p1.Velocity.X = 0;
            p1.Position = new Vector2f(WorldSize - 4, p1.Position.Y);
        }
        if (p1.Position.Y >= WorldSize)
        {
            p1.Velocity.Y = 0;
            p1.Position = new Vector2f(p1.Position.Y, WorldSize - 4);
        }

        if (p1.Position.Y <= 0)
        {
            p1.Velocity.Y = 0;
            p1.Position = new Vector2f(0, p1.Position.Y);
        }
        if (p1.Position.X <= 0)
        {
            p1.Velocity.X = 0;
            p1.Position = new Vector2f(p1.Position.X, 0);
        }
    }

    public float Random() => ran.Next(0, WorldSize);

    public ParticleSpecies Create(int num, Color colour)
    {
        ParticleSpecies species = new ParticleSpecies();

        species.Particles = new SpatialHash(50); //we'll use grids as 50 as i find that works the best for these

        for (int i = 0; i < num; ++i)
        {
            var obj = new SolidParticle(1)
            {
                Color = colour,
                Position = new Vector2f(Random(), Random())
            };

            // this should force its update by default (it should be a single point in the spatial partitioning hashmap)
            species.Particles.AddObject(obj);
        }

        species.Colour = colour;

        // add new species to the species hashset
        ParticleSpecies.Add(species);

        return species;
    }

    public void SetInteraction(int speciesIndex1, int speciesIndex2, float g)
    {
        var key = speciesIndex1 < speciesIndex2 ? (speciesIndex1, speciesIndex2) : (speciesIndex2, speciesIndex1);
        interactMatrices[key] = g;
    }

    public float? GetInteraction(int speciesIndex1, int speciesIndex2)
    {
        var key = speciesIndex1 < speciesIndex2 ? (speciesIndex1, speciesIndex2) : (speciesIndex2, speciesIndex1);
        if (interactMatrices.TryGetValue(key, out float g))
            return g;

        return null;
    }

    // called 20 times a second at physics step
    public void DoRuleMatrix()
    {
        for (int i = 0; i < ParticleSpecies.Count; i++)
        {
            for (int i2 = 0; i2 < ParticleSpecies.Count; i2++)
            {
            redo:
                float? g = GetInteraction(i, i2);

                if (!g.HasValue)
                {
                    SetInteraction(i, i2, (250 - Random()) / WorldSize * 2);
                    Console.WriteLine($"{i}:{i2} {GetInteraction(i, i2)}");
                    goto redo;
                }

                Rule(ParticleSpecies[i], ParticleSpecies[i2], g.Value);
            }
        }
    }
}