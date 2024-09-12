using System.Collections;
using System.Collections.Generic;

class ConcurrentHashset<T> : IEnumerable<T>
{
    private readonly HashSet<T> _set = new HashSet<T>();
    private readonly object _lock = new object();

    public bool Add(T item)
    {
        lock (_lock)
        {
            return _set.Add(item);
        }
    }

    public bool Remove(T item)
    {
        lock (_lock)
        {
            return _set.Remove(item);
        }
    }

    public bool Contains(T item)
    {
        lock (_lock)
        {
            return _set.Contains(item);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _set.Clear();
        }
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _set.Count;
            }
        }
    }

    // some utils i might need later
    public IEnumerable<T> ToList()
    {
        lock (_lock)
        {
            return new List<T>(_set);
        }
    }

    // loopable
    public IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;

        lock (_lock)
        {
            snapshot = new List<T>(_set);
        }

        return snapshot.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}