# semester3-arbeidskrav1

# Phonebook Search and Sorting

## Overview

This project is a C# console application developed as part of the **Arbeidskrav 1** assignment. It implements and compares manual searching and sorting algorithms on a phonebook dataset without using .NET's built-in sorting or searching methods.

The application loads contacts from a CSV file, performs linear and binary searches, sorts contacts using two different algorithms, and reports comparison and move counts to evaluate algorithm performance.

---

## Features

* Load contacts from `phonebook.csv`
* Linear search by:

    * First Name
    * Last Name
    * Mobile Number
* Sort contacts using:

    * Insertion Sort
    * Merge Sort
* Sort in:

    * Ascending order
    * Descending order
* Binary search on sorted data
* Comparison and move counting for algorithm analysis
* Binary search correctness tests
* LINQ-based correctness oracle (used only for testing)

---

## Project Structure

```text
Arbeidskrav1/
│
├── Contact.cs
├── Phonebook.cs
├── Sorting.cs
├── ContactComparer.cs
├── Program.cs
├── Field.cs
├── phonebook.csv
├── report.md
└── README.md
```

---

## Algorithms Implemented

### Searching

* Linear Search
* Binary Search (Lower Bound implementation)

### Sorting

* Insertion Sort
* Merge Sort

Both sorting algorithms are implemented manually without using:

* `Array.Sort()`
* `OrderBy()`

The search algorithms do not use:

* `Array.BinarySearch()`
* `IndexOf()`
* `Contains()`

---

## Running the Project

### Requirements

* .NET SDK 10.0 

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run
```

Ensure that `phonebook.csv` is available in the output directory or project directory before running the application.

---

## Program Output

The application prints:

1. Number of contacts loaded
2. Linear search results
3. Sorting performance for Insertion Sort and Merge Sort
4. Binary search results
5. Eight binary search correctness tests
6. Comparison and move counts
7. Correctness verification using LINQ (testing only)

---

## Design

The project follows an object-oriented design.

* `Contact` stores phonebook data.
* `Phonebook` manages loading, searching, and sorting contacts.
* `ContactComparer` performs comparisons for different fields and sort orders.
* `CountingComparer<T>` counts comparisons made by the algorithms.
* `Sorting` contains generic implementations of Insertion Sort and Merge Sort.

The comparison logic is centralized in `ContactComparer`, allowing the same sorting and searching code to work with different fields and sort directions.

---

## Complexity

| Algorithm      | Best       | Average    | Worst      | Extra Space |
| -------------- | ---------- | ---------- | ---------- | ----------- |
| Linear Search  | O(n)       | O(n)       | O(n)       | O(k)        |
| Binary Search  | O(log n)   | O(log n)   | O(log n)   | O(1)        |
| Insertion Sort | O(n)       | O(n²)      | O(n²)      | O(1)        |
| Merge Sort     | O(n log n) | O(n log n) | O(n log n) | O(n)        |

---

## AI Usage

AI tools were used as development aids during this project.

**Claude** and **ChatGPT** were used to:

* Discuss algorithm design and implementation ideas.
* Review code for correctness and identify potential improvements.
* Identify missing test cases and suggest additional edge-case testing.
* Improve code comments, documentation, and report wording.
* Help verify that the implementation satisfied the assignment requirements.
* Explain algorithm behaviour, complexity analysis, and help compare measured results with theoretical expectations.


---

## Notes

* Searching is case-insensitive.
* Binary Search assumes the phonebook has already been sorted in ascending order on the same field.
* The LINQ-based correctness checks are included only for verification and are **not** part of the algorithm implementations.

---

## Author

Submitted as part of the **Arbeidskrav 1** assignment.
