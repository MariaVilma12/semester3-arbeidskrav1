namespace Arbeidskrav1;

/// <summary>
/// Compares two contacts on one field, case-insensitively, in the requested
/// direction. This is the single place that knows how to turn a Field and a
/// SortOrder into an actual ordering, so Sort and BinarySearch both build on
/// it instead of duplicating comparison logic.
/// </summary>
public class ContactComparer : IComparer<Contact>
{
    private readonly Field _field;
    private readonly int _direction;

    public ContactComparer(Field field, SortOrder order)
    {
        _field = field;
        _direction = order == SortOrder.Ascending ? 1 : -1;
    }

    public int Compare(Contact? a, Contact? b)
    {
        if (a is null || b is null)
            throw new ArgumentNullException("Cannot compare a null contact.");

        int result = string.Compare(a.KeyFor(_field), b.KeyFor(_field),
            StringComparison.OrdinalIgnoreCase);
        return result * _direction;
    }
}

/// <summary>
/// Wraps any IComparer&lt;T&gt; and counts how many times Compare is called.
/// The sorting algorithms in <see cref="Sorting"/> take a plain
/// IComparer&lt;T&gt;, so wrapping the comparer (rather than hard-coding a
/// counter inside each algorithm) lets counting be added without touching
/// the algorithms themselves.
/// </summary>
public class CountingComparer<T> : IComparer<T>
{
    private readonly IComparer<T> _inner;

    public int Comparisons { get; private set; }

    public CountingComparer(IComparer<T> inner)
    {
        _inner = inner;
    }

    public int Compare(T? x, T? y)
    {
        Comparisons++;
        return _inner.Compare(x!, y!);
    }

    public void Reset() => Comparisons = 0;
}