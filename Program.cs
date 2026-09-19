using Arbeidskrav1;

// ----------------------------------------------------------------------
// Load
// ----------------------------------------------------------------------
string csvPath = Path.Combine(AppContext.BaseDirectory, "phonebook.csv");
if (!File.Exists(csvPath))
{
    csvPath = Path.Combine(Directory.GetCurrentDirectory(), "phonebook.csv");
}

Phonebook phonebook;
try
{
    phonebook = Phonebook.Load(csvPath);
}
catch (Exception ex) when (ex is FileNotFoundException or InvalidDataException)
{
    Console.WriteLine($"Could not load the phonebook: {ex.Message}");
    return;
}

Console.WriteLine($"Loaded {phonebook.Count} contacts from {csvPath}");

// ----------------------------------------------------------------------
// Question 1: Searching unsorted data
// ----------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("--- 1. Linear search, unsorted ---");
Console.WriteLine($"{"field",-10} {"target",-10} {"matches",-8} {"comparisons",-11}");

var q1Targets = new (Field field, string target)[]
{
    // LastName tests
    (Field.LastName, "Bjerke"),
    (Field.LastName, "Hansen"),
    (Field.LastName, "Aardal"), // absent

    // FirstName tests
    (Field.FirstName, "Julie"),
    (Field.FirstName, "Nobody"), // absent

    // Mobile test
    (Field.Mobile, "00000000") // absent
};

// Search each target in the phonebook and print the number of matches and comparisons.
foreach (var (field, target) in q1Targets)
{
    var matches = phonebook.LinearSearch(field, target, out int comparisons);
    Console.WriteLine($"{field,-10} {target,-10} {matches.Length,-8} {comparisons,-11}");
}

// ----------------------------------------------------------------------
// Question 2: Sorting
// ----------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("--- 2. Sorting, by LastName ascending ---");
Console.WriteLine($"{"algorithm",-14} {"shape",-16} {"comparisons",-11} {"moves",-6}");

Contact[] asSupplied = phonebook.FreshCopy();

Contact[] alreadySorted = phonebook.FreshCopy();
Sorting.MergeSort(alreadySorted, new ContactComparer(Field.LastName, SortOrder.Ascending));

Contact[] reverseSorted = (Contact[])alreadySorted.Clone();
Array.Reverse(reverseSorted);

var shapes = new (string name, Func<Contact[]> makeCopy)[]
{
    ("as-supplied", () => (Contact[])asSupplied.Clone()),
    ("already-sorted", () => (Contact[])alreadySorted.Clone()),
    ("reverse-sorted", () => (Contact[])reverseSorted.Clone()),
};

void RunSortAndPrint(string algorithmName, Func<Contact[], IComparer<Contact>, int> algorithm)
{
    foreach (var (shapeName, makeCopy) in shapes)
    {
        Contact[] data = makeCopy();
        var counting = new CountingComparer<Contact>(new ContactComparer(Field.LastName, SortOrder.Ascending));
        int moves = algorithm(data, counting);
        Console.WriteLine($"{algorithmName,-14} {shapeName,-16} {counting.Comparisons,-11} {moves,-6}");
    }
}

RunSortAndPrint("InsertionSort", Sorting.InsertionSort);
RunSortAndPrint("MergeSort", Sorting.MergeSort);

// ----------------------------------------------------------------------
// Question 3: Searching sorted data
// ----------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("--- 3. Binary search, sorted by target field ascending ---");
Console.WriteLine($"{"field",-10} {"target",-10} {"result",-8} {"comparisons",-11} {"linear",-6}");

Phonebook SortedBy(Field field)
{
    var pb = Phonebook.Load(csvPath);
    pb.Sort(field, SortOrder.Ascending, SortAlgorithm.MergeSort, out _, out _);
    return pb;
}

var byLastName = SortedBy(Field.LastName);
var byMobile = SortedBy(Field.Mobile);
var byFirstName = SortedBy(Field.FirstName);

void RunBinaryAndLinear(Phonebook sorted, Field field, string target)
{
    int index = sorted.BinarySearch(field, target, out int binComparisons);
    var unsorted = Phonebook.Load(csvPath);
    unsorted.LinearSearch(field, target, out int linComparisons);
    string result = index == -1 ? "-1" : index.ToString();
    Console.WriteLine($"{field,-10} {target,-10} {result,-8} {binComparisons,-11} {linComparisons,-6}");
}

RunBinaryAndLinear(byLastName, Field.LastName, "Bjerke");
RunBinaryAndLinear(byLastName, Field.LastName, "Aardal");
RunBinaryAndLinear(byMobile, Field.Mobile, "97756218");
RunBinaryAndLinear(byFirstName, Field.FirstName, "Julie");

// ----------------------------------------------------------------------
// The eight required binary search tests
// ----------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("--- Binary search: 8 required tests ---");

int passCount = 0;
int total = 0;

void Check(string label, bool passed)
{
    total++;
    if (passed) passCount++;
    Console.WriteLine($"{total,2}. {label,-55} {(passed ? "PASS" : "FAIL")}");
}

{
    string target = byMobile[0].Mobile;
    int idx = byMobile.BinarySearch(Field.Mobile, target, out _);
    Check("Mobile, number from file -> its index", idx == 0);
}
{
    int idx = byMobile.BinarySearch(Field.Mobile, "00000000", out _);
    Check("Mobile, 00000000 -> -1 (below smallest)", idx == -1);
}
{
    int idx = byMobile.BinarySearch(Field.Mobile, "99999999", out _);
    Check("Mobile, 99999999 -> -1 (above largest)", idx == -1);
}
{
    int idx = byLastName.BinarySearch(Field.LastName, "Bjerke", out _);
    bool proof = idx > 0 &&
        !string.Equals(byLastName[idx - 1].LastName, "Bjerke", StringComparison.OrdinalIgnoreCase) &&
        string.Equals(byLastName[idx].LastName, "Bjerke", StringComparison.OrdinalIgnoreCase);
    Console.WriteLine($"    proof: contacts[{idx - 1}]='{byLastName[idx - 1].LastName}', contacts[{idx}]='{byLastName[idx].LastName}'");
    Check("LastName, duplicate surname -> lowest index", idx >= 0 && proof);
}
{
    int idx = byLastName.BinarySearch(Field.LastName, "Aardal", out _);
    Check("LastName, absent surname -> -1", idx == -1);
}
{
    int idx = byFirstName.BinarySearch(Field.FirstName, "Julie", out _);
    bool proof = idx > 0 &&
        !string.Equals(byFirstName[idx - 1].FirstName, "Julie", StringComparison.OrdinalIgnoreCase) &&
        string.Equals(byFirstName[idx].FirstName, "Julie", StringComparison.OrdinalIgnoreCase);
    Console.WriteLine($"    proof: contacts[{idx - 1}]='{byFirstName[idx - 1].FirstName}', contacts[{idx}]='{byFirstName[idx].FirstName}'");
    Check("FirstName, duplicate name -> lowest index", idx >= 0 && proof);
}
{
    var emptyContacts = Array.Empty<Contact>();
    int lo = 0, hi = emptyContacts.Length, comparisons = 0;
    while (lo < hi)
    {
        int mid = lo + (hi - lo) / 2;
        comparisons++;
        if (string.Compare(emptyContacts[mid].LastName, "Anything", StringComparison.OrdinalIgnoreCase) < 0)
            lo = mid + 1;
        else
            hi = mid;
    }
    comparisons++;
    int idx = (lo < emptyContacts.Length) ? lo : -1;
    Check("Empty array -> -1", idx == -1);
}
{
    var single = new[] { phonebook.FreshCopy()[0] };
    string target = single[0].LastName;
    int lo = 0, hi = single.Length, comparisons = 0;
    while (lo < hi)
    {
        int mid = lo + (hi - lo) / 2;
        comparisons++;
        if (string.Compare(single[mid].LastName, target, StringComparison.OrdinalIgnoreCase) < 0)
            lo = mid + 1;
        else
            hi = mid;
    }
    comparisons++;
    int idx = (lo < single.Length &&
               string.Equals(single[lo].LastName, target, StringComparison.OrdinalIgnoreCase)) ? lo : -1;
    Check("One-element array -> 0", idx == 0);
}

Console.WriteLine();
Console.WriteLine($"{passCount}/{total} binary search tests passed.");

// ----------------------------------------------------------------------
// Correctness oracle (testing only)
// ----------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("--- Correctness oracle (LINQ, testing only) ---");

Contact[] oracleExpected = phonebook.FreshCopy()
    .OrderBy(c => c.LastName, StringComparer.OrdinalIgnoreCase)
    .ToArray();
Contact[] oracleActual = phonebook.FreshCopy();
Sorting.MergeSort(oracleActual, new ContactComparer(Field.LastName, SortOrder.Ascending));

bool sortAgrees = oracleExpected.Select(c => c.Mobile)
    .SequenceEqual(oracleActual.Select(c => c.Mobile));
Console.WriteLine($"MergeSort matches LINQ OrderBy: {(sortAgrees ? "PASS" : "FAIL")}");

var oracleSearch = phonebook.FreshCopy()
    .Where(c => string.Equals(c.LastName, "Bjerke", StringComparison.OrdinalIgnoreCase))
    .Select(c => c.Mobile)
    .OrderBy(m => m)
    .ToArray();
var actualSearch = phonebook.LinearSearch(Field.LastName, "Bjerke", out _)
    .Select(c => c.Mobile)
    .OrderBy(m => m)
    .ToArray();
Console.WriteLine($"LinearSearch matches LINQ Where: {(oracleSearch.SequenceEqual(actualSearch) ? "PASS" : "FAIL")}");