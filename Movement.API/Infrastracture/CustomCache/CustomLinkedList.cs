namespace Movement.API.Infrastructure.CustomCache;

internal sealed class CustomLinkedList<TKey, TValue>
{
    public CustomNode<TKey, TValue>? Head { get; private set; }
    public CustomNode<TKey, TValue>? Tail { get; private set; }

    public void AddFirst(CustomNode<TKey, TValue> node)
    {
        if (Head is null)
        {
            Head = Tail = node;
        }
        else
        {
            node.Next = Head;
            Head.Previous = node;
            Head = node;
        }
    }

    public void Remove(CustomNode<TKey, TValue> node)
    {
        if (node.Previous is not null)
            node.Previous.Next = node.Next;
        else
            Head = node.Next;

        if (node.Next is not null)
            node.Next.Previous = node.Previous;
        else
            Tail = node.Previous;

        node.Previous = null;
        node.Next = null;
    }

    public CustomNode<TKey, TValue>? RemoveLast()
    {
        if (Tail is null) return null;
        var tail = Tail;
        Remove(tail);
        return tail;
    }

    public void MoveToFront(CustomNode<TKey, TValue> node)
    {
        Remove(node);
        AddFirst(node);
    }

    public void Clear()
    {
        Head = null;
        Tail = null;
    }
}
