namespace Authorisation.Models
{
    public class Organisation
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public Organisation(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}