
namespace Requests.Domain
{
    public class Person 
    {
        // properties
        public int ID { get; private set; }
        public string FirstName { get; private set;}
        public string LastName { get; private set;}
        public string Salutation { get; private set;}

        // ctors
        public Person(int  Id, string firstName, string lastName, string salutation = "")
        {
            ID = Id;
            Initialize(firstName, lastName, salutation);
        }
        
        public Person(string firstName, string lastName, string salutation = "")
        {
            Initialize(firstName, lastName, salutation);
        }

        public Person()
        {
        }

        // methods
        public  string FullName()
        {
            var fullName =  $"{Salutation} {FirstName} {LastName}";
            return fullName.Trim().Replace("  ", " ");
        }

        public void Initialize(string firstName, string lastName, string salutation = "")
        {
            FirstName = firstName;
            LastName = lastName;
            Salutation = salutation;           
        }
    }
}