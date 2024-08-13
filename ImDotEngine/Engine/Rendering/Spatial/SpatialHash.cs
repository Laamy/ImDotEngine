using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

class SpatialHash
{
    private readonly int cellSize;
    private readonly Dictionary<int, HashSet<SolidActor>> hashGrid;

    public int Count { get; private set; } = 0;
    public int Hashes { get; private set; } = 0;

    public SpatialHash(int cellSize)
    {
        this.cellSize = cellSize;
        this.hashGrid = new Dictionary<int, HashSet<SolidActor>>();
    }

    private int HashPosition(Vector2f position)
    {
        int x = (int)Math.Floor(position.X / cellSize);
        int y = (int)Math.Floor(position.Y / cellSize);
        return x + y * 73856093;
    }

    private IEnumerable<int> GetHashesForBounds(FloatRect bounds)
    {
        int minX = (int)Math.Floor(bounds.Left / cellSize);
        int maxX = (int)Math.Floor((bounds.Left + bounds.Width) / cellSize);
        int minY = (int)Math.Floor(bounds.Top / cellSize);
        int maxY = (int)Math.Floor((bounds.Top + bounds.Height) / cellSize);

        for (int x = minX; x <= maxX; ++x)
        {
            for (int y = minY; y <= maxY; ++y)
            {
                yield return x + y * 73856093;
            }
        }
    }

    public void AddObject(SolidActor obj)
    {
        var bounds = new FloatRect(obj.GetPosition(), obj.GetSize());
        var added = false;

        foreach (var hash in GetHashesForBounds(bounds))
        {
            if (!hashGrid.ContainsKey(hash))
            {
                hashGrid[hash] = new HashSet<SolidActor>();
                Hashes++;
            }

            if (hashGrid[hash].Add(obj))
                added = true;
        }

        if (added)
            Count++;

        if (!added)
        {
            // If the object was not added, remove it (could happen if object was already present)
            foreach (var hash in GetHashesForBounds(bounds))
            {
                if (hashGrid.ContainsKey(hash))
                {
                    hashGrid[hash].Remove(obj);
                    if (hashGrid[hash].Count == 0)
                    {
                        hashGrid.Remove(hash);
                        Hashes--;
                    }
                }
            }
        }
    }

    public void RemoveObject(SolidActor obj)
    {
        var bounds = new FloatRect(obj.GetPosition(), obj.GetSize());
        foreach (var hash in GetHashesForBounds(bounds))
        {
            if (hashGrid.ContainsKey(hash))
            {
                if (hashGrid[hash].Remove(obj))
                {
                    if (hashGrid[hash].Count == 0)
                    {
                        hashGrid.Remove(hash);
                        Hashes--;
                    }
                }
            }
        }
        Count--;
    }

    public IEnumerable<SolidActor> GetObjectsInBounds(FloatRect bounds)
    {
        var result = new HashSet<SolidActor>();
        foreach (var hash in GetHashesForBounds(bounds))
        {
            if (hashGrid.ContainsKey(hash))
            {
                foreach (var obj in hashGrid[hash])
                {
                    var objBounds = new FloatRect(obj.GetPosition(), obj.GetSize());
                    if (objBounds.Intersects(bounds))
                    {
                        result.Add(obj);
                    }
                }
            }
        }
        return result;
    }

    public IEnumerable<SolidActor> GetNearbyObjects(Vector2f position, int radius)
    {
        var bounds = new FloatRect(position.X - radius, position.Y - radius, radius * 2, radius * 2);
        return GetObjectsInBounds(bounds);
    }
}