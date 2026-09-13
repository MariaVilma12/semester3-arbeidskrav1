namespace Arbeidskrav1;

/// <summary>
/// A single phonebook entry. Data is fully encapsulated: the fields are
/// private, and callers only ever see them through the public properties.
/// A Contact never prints anything itself; formatting is the caller's job.
/// </summary>
public class Contact
{
    private readonly string _firstName;
    private readonly string _lastName;
    private readonly string _mobile;
    private readonly string _birthday;
    private readonly string _street;
    private readonly string _city;

    public string FirstName => _firstName;
    public string LastName => _lastName;
    public string Mobile => _mobile;
    public string Birthday => _birthday;
    public string Street => _street;
    public string City => _city;

    public Contact(string firstName, string lastName, string mobile,
        string birthday, string street, string city)
    {
        _firstName = firstName;
        _lastName = lastName;
        _mobile = mobile;
        _birthday = birthday;
        _street = street;
        _city = city;
    }

    /// <summary>
    /// Returns the value of the given field, so that searching and sorting
    /// code can stay generic over Field instead of switching on it in three
    /// different places.
    /// </summary>
    public string KeyFor(Field field) => field switch
    {
        Field.FirstName => _firstName,
        Field.LastName => _lastName,
        Field.Mobile => _mobile,
        _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Unknown field.")
    };

    public override string ToString() => $"{_firstName} {_lastName} ({_mobile})";
}