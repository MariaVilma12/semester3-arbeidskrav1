namespace Arbeidskrav1;

/// <summary>
/// Two hand-written sorting algorithms, generic over the element type so the
/// same code sorts contacts on any field without being copied per field.
/// Neither method calls Array.Sort, OrderBy, or any other built-in sort.
/// </summary>
public static class Sorting
{
    /// <summary>
    /// Insertion Sort. Sorts <paramref name="items"/> in place.
    /// Time complexity: best O(n) (already sorted, one comparison per
    /// element, no shifts); average O(n^2); worst O(n^2) (reverse sorted).
    /// Space complexity: O(1) extra (one temporary "key" slot).
    /// </summary>
    public static int InsertionSort<T>(T[] items, IComparer<T> comparer)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));
        if (comparer is null) throw new ArgumentNullException(nameof(comparer));

        int moves = 0;
        for (int i = 1; i < items.Length; i++)
        {
            T key = items[i];
            int j = i - 1;

            // comparer.Compare counts each call itself (see CountingComparer)
            while (j >= 0 && comparer.Compare(items[j], key) > 0)
            {
                items[j + 1] = items[j];
                moves++;
                j--;
            }

            items[j + 1] = key;
        }
        return moves;
    }

    /// <summary>
    /// Merge Sort. Sorts <paramref name="items"/> in place, but unlike the
    /// other algorithm it needs a temporary buffer while merging two
    /// already-sorted halves back together; that buffer is discarded once
    /// the merge step finishes.
    /// Time complexity: best, average and worst are all O(n log n) - the
    /// recursion always splits the array in half regardless of its initial
    /// order, and every merge step is a single linear pass.
    /// Space complexity: O(n) for the temporary buffer used during merging.
    /// </summary>
    public static int MergeSort<T>(T[] items, IComparer<T> comparer)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));
        if (comparer is null) throw new ArgumentNullException(nameof(comparer));

        int moves = 0;
        MergeSortRange(items, 0, items.Length, comparer, ref moves);
        return moves;
    }

    private static void MergeSortRange<T>(T[] items, int lo, int hi,
                                           IComparer<T> comparer, ref int moves)
    {
        if (hi - lo <= 1) return; // 0 or 1 elements: already sorted

        int mid = lo + (hi - lo) / 2;
        MergeSortRange(items, lo, mid, comparer, ref moves);
        MergeSortRange(items, mid, hi, comparer, ref moves);
        Merge(items, lo, mid, hi, comparer, ref moves);
    }

    private static void Merge<T>(T[] items, int lo, int mid, int hi,
                                  IComparer<T> comparer, ref int moves)
    {
        T[] temp = new T[hi - lo];
        Array.Copy(items, lo, temp, 0, hi - lo);

        int leftLen = mid - lo;
        int rightLen = hi - mid;
        int i = 0, j = leftLen, k = lo;

        while (i < leftLen && j < leftLen + rightLen)
        {
            if (comparer.Compare(temp[i], temp[j]) <= 0)
            {
                items[k] = temp[i];
                i++;
            }
            else
            {
                items[k] = temp[j];
                j++;
            }
            moves++;
            k++;
        }

        while (i < leftLen)
        {
            items[k] = temp[i];
            moves++;
            i++;
            k++;
        }

        while (j < leftLen + rightLen)
        {
            items[k] = temp[j];
            moves++;
            j++;
            k++;
        }
    }
}