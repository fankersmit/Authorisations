namespace Authorisations.Models
{
    public class PersonModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        private string Salutation { get; set; }
    }
}