namespace Requests.Domain
{
    public interface IPersonModel
    {
        string FirstName { get; }
        string LastName { get; }
        string FullName { get; }
        string Salutation { get; }
    }
}