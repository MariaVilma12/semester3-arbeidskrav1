# Searching and Sorting: Report

| | |
|---|---|
| Name | [Your Name] |
| Date | [Submission date] |
| Data | phonebook.csv, 200 contacts |

---

## 1. Searching unsorted data

| Field | Target | Case | Matches | Comparisons |
|---|---|---|---|---|
| LastName | Bjerke | first record (best case) | 9 | 200 |
| LastName | Hansen | last record (worst case) | 7 | 200 |
| LastName | Aardal | absent value | 0 | 200 |
| Mobile | 00000000 | absent value | 0 | 200 |

**Reflection.** All four rows come out to 200 comparisons, even the "best
case" one. My LinearSearch has to return every match, not just the first
one, so it can't stop early - even if it finds Bjerke at index 0, it still
has to check the rest in case there's another Bjerke later. So the usual
O(1) best case / O(n) worst case split doesn't really apply here, it's O(n)
every time. An absent value also costs 200 comparisons, for the same
reason - the only way to know something's absent is to check the whole
array and find nothing. And finding 9 matches doesn't cost more than
finding 0 - the loop runs the same number of times either way, matching
just decides whether that comparison also adds something to the results
list.

## 2. Sorting

| Algorithm | Input shape | Comparisons | Swaps or moves |
|---|---|---|---|
| InsertionSort | as supplied | 9691 | 9494 |
| InsertionSort | already sorted | 199 | 0 |
| InsertionSort | reverse sorted | 19571 | 19411 |
| MergeSort | as supplied | 1287 | 1544 |
| MergeSort | already sorted | 732 | 1544 |
| MergeSort | reverse sorted | 877 | 1544 |

**Reflection.** InsertionSort is the one that really reacts to input shape.
On already-sorted data it only needed 199 comparisons and made 0 moves -
each new element just gets compared once to the one before it, finds it's
already in the right spot, and moves on. On reverse-sorted data it's the
opposite: 19571 comparisons and 19411 moves, since every single element has
to be shifted all the way to the front. The as-supplied figure (9691) sits
right in between, close to what you'd expect from random data.
MergeSort barely reacts to shape at all - comparisons only range from 732
to 1287 across all three, and the move count is identical (1544) every
time, since it always splits the array in half and merges regardless of
what order things started in.
This matches the theory: InsertionSort is O(n) best case, O(n²) average and
worst - and the numbers back that up exactly, 199 vs 9691 vs 19571.
MergeSort is O(n log n) no matter what, and that's basically what we see -
nowhere near the swings InsertionSort has.

## 3. Searching sorted data

| Field | Target | Result | Comparisons |
|---|---|---|---|
| LastName | Bjerke (appears 9 times) | 29 | 9 |
| LastName | Aardal (absent) | -1 | 9 |
| Mobile | 97756218 (from file) | 179 | 9 |
| FirstName | Julie (appears 7 times) | 75 | 8 |

Linear search on the same targets, for comparison:

| Target | Comparisons (linear) | Comparisons (binary) |
|---|---|---|
| LastName Bjerke | 200 | 9 |
| LastName Aardal | 200 | 9 |
| Mobile 97756218 | 200 | 9 |
| FirstName Julie | 200 | 8 |

**Reflection.** Binary search took 8-9 comparisons on 200 contacts, which
lines up with log2(200) = 7.64 (the extra comparison is the final equality
check once the range is down to one spot). To guarantee I get the first
occurrence of a duplicate, my binary search only moves past values that are
strictly less than the target, never past ones that are equal to it - so it
can only land on the earliest match. I checked this by printing the entry
right before the one it returned and confirming it's a different value.
Sorting by LastName cost 1287 comparisons up front (MergeSort, on the
as-supplied data). After that, each binary search costs 9 comparisons
instead of 200 for linear search - a saving of 191 per search. So sorting
first only pays off after about 1287 / 191 ≈ 7 searches on the same field.
Fewer than that and you'd have been better off just doing linear search.

## 4. Insight

What stood out most to me is how differently InsertionSort and MergeSort
react to the same input. InsertionSort can be really fast (199 comparisons
on sorted data) or really slow (19571 on reverse-sorted) depending purely
on luck. MergeSort barely cares either way. So "best case" and "worst
case" aren't just theory - they showed up directly in my own numbers as a
100x difference for InsertionSort. The same idea applies on the searching
side too: binary search is over 20x cheaper per lookup than linear search,
but you only get that if you've already paid to sort the data, and that
only pays off if you're going to search it several times. So which
algorithm is "best" really depends on the data you're actually working
with, not just which one sounds more advanced.