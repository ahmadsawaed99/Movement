namespace Movement.API.Infrastructure.CustomCache;

internal sealed class CustomNode<TKey, TValue>
{
    public TKey Key { get; set; }
    public TValue Value { get; set; }
    public CustomNode<TKey, TValue>? Previous { get; set; }
    public CustomNode<TKey, TValue>? Next { get; set; }

    public CustomNode(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
