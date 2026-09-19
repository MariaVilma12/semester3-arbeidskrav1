# Searching and Sorting: Report

|      |                             |
| ---- |-----------------------------|
| Name | Maria Vilma                 |
| Date | 24-09-2026                  |
| Data | phonebook.csv, 200 contacts |

---

## 1. Searching unsorted data

| Field    | Target   | Case                     | Matches | Comparisons |
| -------- | -------- | ------------------------ | ------- | ----------- |
| LastName | Bjerke   | first record (best case) | 9       | 200         |
| LastName | Hansen   | last record (worst case) | 7       | 200         |
| LastName | Aardal   | absent value             | 0       | 200         |
| Mobile   | 00000000 | absent value             | 0       | 200         |

**Reflection.** All four searches required **200 comparisons** because the `LinearSearch` implementation returns every matching contact rather than stopping after the first match. Even when the first contact matches, the algorithm must continue scanning the entire array to find any duplicates. An absent value also requires checking all 200 contacts before the algorithm can conclude that no match exists. Therefore, this implementation runs in **O(n)** time for every search. Finding nine matches costs no more comparisons than finding none because every contact is examined.

---

## 2. Sorting

| Algorithm     | Input shape    | Comparisons | Swaps or moves |
| ------------- | -------------- | ----------- | -------------- |
| InsertionSort | as supplied    | 9691        | 9494           |
| InsertionSort | already sorted | 199         | 0              |
| InsertionSort | reverse sorted | 19571       | 19411          |
| MergeSort     | as supplied    | 1287        | 1544           |
| MergeSort     | already sorted | 732         | 1544           |
| MergeSort     | reverse sorted | 877         | 1544           |

**Reflection.** InsertionSort is strongly affected by the initial ordering of the data. On already sorted data, it required only **199 comparisons** and **0 moves** because each new element was already in the correct position. On reverse-sorted data, it required **19,571 comparisons** and **19,411 moves** because each element had to be shifted repeatedly to reach its correct position. The as-supplied data required **9,691 comparisons**, which is consistent with average-case behaviour.

MergeSort is much less affected by the initial ordering of the data. The number of comparisons ranged from **732** to **1,287**, while the move count remained constant at **1,544** because the algorithm always divides the array into smaller subarrays and merges them regardless of the original order.

These results agree with the theoretical complexities. InsertionSort has a best-case complexity of **O(n)** and average- and worst-case complexities of **O(n²)**. MergeSort has a complexity of **O(n log n)** in the best, average, and worst cases, which matches the measured results.

---

## 3. Searching sorted data

| Field     | Target                   | Result | Comparisons |
| --------- | ------------------------ | ------ | ----------- |
| LastName  | Bjerke (appears 9 times) | 29     | 9           |
| LastName  | Aardal (absent)          | -1     | 9           |
| Mobile    | 97756218 (from file)     | 179    | 9           |
| FirstName | Julie (appears 7 times)  | 75     | 8           |

Linear search on the same targets, for comparison:

| Target          | Comparisons (linear) | Comparisons (binary) |
| --------------- | -------------------- | -------------------- |
| LastName Bjerke | 200                  | 9                    |
| LastName Aardal | 200                  | 9                    |
| Mobile 97756218 | 200                  | 9                    |
| FirstName Julie | 200                  | 8                    |

**Reflection.** Binary search required only **8–9 comparisons** for 200 contacts, which is close to **log₂(200) ≈ 7.64**. The extra comparison comes from the final equality check after the search range has been reduced to one position. To guarantee the first occurrence of a duplicate, the implementation uses a lower-bound approach, moving past values that are strictly less than the target but never past equal values. This ensures the search always returns the lowest index of a matching value.

Sorting the phonebook by LastName using MergeSort required **1,287 comparisons**. After sorting, each binary search required only **8–9 comparisons**, compared with **200** for linear search. This saves approximately **191 comparisons** per search, so the initial sorting cost is recovered after about **seven searches** on the same field.

---

## 4. Insight

The most useful lesson from this assignment is that the performance of an algorithm depends on both the data and the task. The results showed that InsertionSort performed extremely well on already sorted data (**199 comparisons**) but very poorly on reverse-sorted data (**19,571 comparisons**), while MergeSort remained consistent regardless of input order. Similarly, binary search required only **8–9 comparisons**, compared with **200** for linear search, but this advantage was only achieved after paying the initial sorting cost. These results demonstrate that the most suitable algorithm depends on how the data will be used rather than on a single algorithm always being the best.
with, not just which one sounds more advanced.