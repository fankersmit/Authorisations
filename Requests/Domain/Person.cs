using System.Text;

namespace Requests.Domain
{
    public class Person : IPersonModel
    {
        // ctors
        public Person(string firstName, string lastName, string salutation = "")
        {
            FirstName = firstName;
            LastName = lastName;
            Salutation = salutation;
        }

        private Person()
        {
        }

        // properties
        public int PersonId { get; private set; }

        public string FirstName { get; private set;}

        public string LastName { get; private set;}

        public string Salutation { get; private set;}

        // methods
        public  string FullName()
        {
            var fullName =  $"{Salutation} {FirstName} {LastName}";
            return fullName.Trim().Replace("  ", " ");
        }
    }
}