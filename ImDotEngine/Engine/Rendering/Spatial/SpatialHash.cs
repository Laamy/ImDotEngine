using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;

// TODO: add a quick way to update/move objects around without reinserting constantly
class SpatialHash
{
    private readonly int cellSize;
    private readonly Dictionary<int, HashSet<SolidActor>> hashGrid;
    private readonly Dictionary<SolidActor, int[]> objectToHashesMap;

    public int Count { get; private set; } = 0;
    public int Hashes { get; private set; } = 0;

    public SpatialHash(int cellSize)
    {
        this.cellSize = cellSize;
        this.hashGrid = new Dictionary<int, HashSet<SolidActor>>();
        this.objectToHashesMap = new Dictionary<SolidActor, int[]>();
    }

    private int HashPosition(Vector2f position)
    {
        int x = (int)Math.Floor(position.X / cellSize);
        int y = (int)Math.Floor(position.Y / cellSize);
        return x + y * 73856093;
    }

    private void GetHashesForBounds(FloatRect bounds, int[] hashes)
    {
        int index = 0;
        int minX = (int)Math.Floor(bounds.Left / cellSize);
        int maxX = (int)Math.Floor((bounds.Left + bounds.Width) / cellSize);
        int minY = (int)Math.Floor(bounds.Top / cellSize);
        int maxY = (int)Math.Floor((bounds.Top + bounds.Height) / cellSize);

        for (int x = minX; x <= maxX; ++x)
        {
            for (int y = minY; y <= maxY; ++y)
            {
                hashes[index++] = x + y * 73856093;
            }
        }
    }

    public void AddObject(SolidActor obj)
    {
        var bounds = new FloatRect(obj.GetPosition(), obj.GetSize());
        var numHashes = GetHashCount(bounds);
        int[] objectHashes = new int[numHashes];

        GetHashesForBounds(bounds, objectHashes);
        bool added = false;

        foreach (var hash in objectHashes)
        {
            if (!hashGrid.ContainsKey(hash))
            {
                hashGrid[hash] = new HashSet<SolidActor>();
                Hashes++;
            }

            if (hashGrid[hash].Add(obj))
            {
                added = true;
                Count++;
            }
        }

        if (added)
            objectToHashesMap[obj] = objectHashes;
    }

    public void RemoveObject(SolidActor obj)
    {
        if (!objectToHashesMap.TryGetValue(obj, out var objectHashes))
            return;

        foreach (var hash in objectHashes)
        {
            if (hashGrid[hash].Remove(obj) && hashGrid[hash].Count == 0)
            {
                hashGrid.Remove(hash);
                Hashes--;
            }
        }

        objectToHashesMap.Remove(obj);
        Count--;
    }

    public void UpdateObject(SolidActor obj)
    {
        RemoveObject(obj);
        AddObject(obj);
    }

    public IEnumerable<SolidActor> GetObjectsInBounds(FloatRect bounds)
    {
        var seenObjects = new HashSet<SolidActor>();
        var numHashes = GetHashCount(bounds);
        int[] hashes = new int[numHashes];

        GetHashesForBounds(bounds, hashes);

        foreach (var hash in hashes)
        {
            if (hashGrid.TryGetValue(hash, out var objects))
            {
                foreach (var obj in objects)
                {
                    if (!seenObjects.Contains(obj) && new FloatRect(obj.GetPosition(), obj.GetSize()).Intersects(bounds))
                    {
                        seenObjects.Add(obj);
                        yield return obj;
                    }
                }
            }
        }
    }

    public IEnumerable<SolidActor> GetNearbyObjects(Vector2f position, int radius)
    {
        var bounds = new FloatRect(position.X - radius, position.Y - radius, radius * 2, radius * 2);
        return GetObjectsInBounds(bounds);
    }

    private int GetHashCount(FloatRect bounds)
    {
        int minX = (int)Math.Floor(bounds.Left / cellSize);
        int maxX = (int)Math.Floor((bounds.Left + bounds.Width) / cellSize);
        int minY = (int)Math.Floor(bounds.Top / cellSize);
        int maxY = (int)Math.Floor((bounds.Top + bounds.Height) / cellSize);

        return (maxX - minX + 1) * (maxY - minY + 1);
    }
}