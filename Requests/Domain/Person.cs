using System.Text;

namespace Requests.Domain
{
    public class Person : IPersonModel
    {
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _salutation;

        // ctors
        public Person(string firstName, string lastName, string salutation = "")
        {
            _firstName = firstName;
            _lastName = lastName;
            _salutation = salutation;
        }

        // properties
        public string FirstName => _firstName;

        public string LastName => _lastName;

        public string FullName => Fullname();

        public string Salutation => _salutation;

        // methods
        private string Fullname()
        {
            var fullName =  $"{Salutation} {FirstName} {LastName}";
            return fullName.Trim().Replace("  ", " ");
        }
    }
}