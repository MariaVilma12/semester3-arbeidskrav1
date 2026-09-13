namespace Arbeidskrav1;

/// <summary>
/// The contact field that a search or sort operates on. Using one enum for
/// both operations means the same value can drive LinearSearch, Sort and
/// BinarySearch without three near-identical code paths.
/// </summary>
public enum Field
{
    FirstName,
    LastName,
    Mobile
}

/// <summary>The direction a sort should run in.</summary>
public enum SortOrder
{
    Ascending,
    Descending
}