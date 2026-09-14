namespace Arbeidskrav1;

/// <summary>
/// Holds the array of contacts and provides the three operations the
/// assignment asks for. Searching and sorting both work on the single
/// underlying array, so a sort genuinely changes what a later search sees.
/// </summary>
public class Phonebook
{
    private Contact[] _contacts;

    public int Count => _contacts.Length;

    private Phonebook(Contact[] contacts)
    {
        _contacts = contacts;
    }

    /// <summary>
    /// Loads contacts from a comma-separated file with the header
    /// FirstName,LastName,Mobile,Birthday,Street,City. No field in the
    /// supplied data contains a comma, so a plain split is enough.
    /// </summary>
    public static Phonebook Load(string csvPath)
    {
        if (!File.Exists(csvPath))
            throw new FileNotFoundException($"Could not find phonebook file at '{csvPath}'.", csvPath);

        var lines = File.ReadAllLines(csvPath);
        if (lines.Length == 0)
            throw new InvalidDataException("Phonebook file is empty.");

        var contacts = new List<Contact>(lines.Length - 1);
        for (int i = 1; i < lines.Length; i++) // skip header row
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var parts = lines[i].Split(',');
            if (parts.Length != 6)
                throw new InvalidDataException(
                    $"Line {i + 1} does not have 6 fields: '{lines[i]}'");

            contacts.Add(new Contact(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]));
        }

        return new Phonebook(contacts.ToArray());
    }

    /// <summary>
    /// Returns a fresh copy of the loaded contacts, in their original order.
    /// Used to reload a clean array before every sorting run, so that one
    /// algorithm's output never becomes another algorithm's "already sorted"
    /// input by accident.
    /// </summary>
    public Contact[] FreshCopy()
    {
        var copy = new Contact[_contacts.Length];
        Array.Copy(_contacts, copy, _contacts.Length);
        return copy;
    }

    /// <summary>
    /// Walks the array from end to end, case-insensitively matching
    /// <paramref name="target"/> against <paramref name="field"/>. Always
    /// scans every element, even after the first match, because the
    /// contract is to return every match, not just one - there is no way to
    /// know a later duplicate does not exist without checking.
    /// Time complexity: O(n) in every case, because every match must be
    /// found. Space complexity: O(k) for k matches returned.
    /// </summary>
    public Contact[] LinearSearch(Field field, string target, out int comparisons)
    {
        if (target is null) throw new ArgumentNullException(nameof(target));

        var matches = new List<Contact>();
        int count = 0;
        foreach (var contact in _contacts)
        {
            count++;
            if (string.Equals(contact.KeyFor(field), target, StringComparison.OrdinalIgnoreCase))
                matches.Add(contact);
        }

        comparisons = count;
        return matches.ToArray();
    }

    /// <summary>
    /// Sorts the underlying array in place by <paramref name="field"/> and
    /// <paramref name="order"/> using <paramref name="algorithm"/>.
    /// Survives empty and single-element arrays without throwing or
    /// reordering anything (both InsertionSort and MergeSort exit before
    /// touching an array with fewer than 2 elements).
    /// </summary>
    public void Sort(Field field, SortOrder order, SortAlgorithm algorithm,
                      out int comparisons, out int moves)
    {
        if (_contacts is null) throw new ArgumentNullException(nameof(_contacts));

        var counting = new CountingComparer<Contact>(new ContactComparer(field, order));

        moves = algorithm switch
        {
            SortAlgorithm.InsertionSort => Sorting.InsertionSort(_contacts, counting),
            SortAlgorithm.MergeSort => Sorting.MergeSort(_contacts, counting),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm))
        };

        comparisons = counting.Comparisons;
    }

    /// <summary>
    /// Binary search using the "lower bound" pattern: it narrows the range
    /// to the first position whose key is not less than the target, then
    /// checks that one position for equality. Because the loop always moves
    /// lo past keys that are strictly less than the target and never past
    /// keys equal to it, the position it lands on is guaranteed to be the
    /// first (lowest-index) occurrence when the target has duplicates - it
    /// never has the chance to stop on a later copy.
    ///
    /// Precondition: the array must already be sorted ascending by
    /// <paramref name="field"/>. This is trusted rather than checked on
    /// every call (checking would itself cost O(n) and defeat the point of
    /// searching in O(log n)); callers are expected to have just sorted by
    /// the same field, which is exactly how this class is used in Program.cs.
    ///
    /// Time complexity: O(log n) in every case. Space complexity: O(1).
    /// </summary>
    public int BinarySearch(Field field, string target, out int comparisons)
    {
        if (target is null) throw new ArgumentNullException(nameof(target));

        int lo = 0, hi = _contacts.Length;
        int count = 0;

        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            count++;
            if (string.Compare(_contacts[mid].KeyFor(field), target,
                                StringComparison.OrdinalIgnoreCase) < 0)
                lo = mid + 1;
            else
                hi = mid;
        }

        count++; // final equality check
        comparisons = count;

        if (lo < _contacts.Length &&
            string.Equals(_contacts[lo].KeyFor(field), target, StringComparison.OrdinalIgnoreCase))
            return lo;

        return -1;
    }

    public Contact this[int index] => _contacts[index];
}

public enum SortAlgorithm
{
    InsertionSort,
    MergeSort
}