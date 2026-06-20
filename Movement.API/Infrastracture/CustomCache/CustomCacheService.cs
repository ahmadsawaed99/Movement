namespace Movement.API.Infrastructure.CustomCache;

/// <summary>
/// Thread-safe, capacity-bounded LRU (Least Recently Used) cache backed by a doubly-linked
/// list and a dictionary for O(1) get, insert, and eviction. Acts as an in-process L2 cache
/// when the distributed Redis cache is unavailable or has no entry for the requested key.
/// </summary>
public sealed class CustomCacheService<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, CustomNode<TKey, TValue>> _map;
    private readonly CustomLinkedList<TKey, TValue> _list;
    private readonly object _lock = new();
    private int _capacity;

    public int Capacity
    {
        get => _capacity;
        set
        {
            if (value < 3 || value > 100)
                throw new ArgumentOutOfRangeException(
                    nameof(Capacity), value,
                    $"Capacity must be in the range [3, 100]. Provided value: {value}.");

            lock (_lock)
            {
                _capacity = value;
                while (_map.Count > _capacity)
                    EvictLRU();
            }
        }
    }

    public int Count { get { lock (_lock) return _map.Count; } }

    public CustomCacheService(int capacity)
    {
        _map = new Dictionary<TKey, CustomNode<TKey, TValue>>();
        _list = new CustomLinkedList<TKey, TValue>();
        Capacity = capacity;
    }

    public TValue? TryGet(TKey key)
    {
        lock (_lock)
        {
            if (!_map.TryGetValue(key, out var node))
            {
                return default;
            }

            _list.MoveToFront(node);
            return node.Value;
        }
    }

    public void AddOrUpdate(TKey key, TValue value)
    {
        lock (_lock)
        {
            if (_map.TryGetValue(key, out var existing))
            {
                existing.Value = value;
                _list.MoveToFront(existing);
                return;
            }

            if (_map.Count == _capacity)
                EvictLRU();

            var node = new CustomNode<TKey, TValue>(key, value);
            _map[key] = node;
            _list.AddFirst(node);
        }
    }

    public bool Remove(TKey key)
    {
        lock (_lock)
        {
            if (!_map.TryGetValue(key, out var node))
                return false;

            _list.Remove(node);
            _map.Remove(key);
            return true;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _list.Clear();
            _map.Clear();
        }
    }

    // Caller must hold _lock
    private void EvictLRU()
    {
        var lru = _list.RemoveLast();
        if (lru is not null)
            _map.Remove(lru.Key);
    }
}
