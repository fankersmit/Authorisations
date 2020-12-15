using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Requests.Domain;
using Requests.Shared.Domain;

namespace RequestsApp.Infrastructure
{
    
    public class RequestDbContext : DbContext
    {
        // ctors
        public RequestDbContext ( DbContextOptions<RequestDbContext>  options ): base (options)
        {
        }

        // repo
        public DbSet<RequestDocument> RequestDocuments { get; set; }
        
        
        // methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new EnumToStringConverter<Commands>();
            
            modelBuilder.Entity<RequestDocument>().HasKey(x => x.ID);
            modelBuilder.Entity<RequestDocument>().Ignore(b => b.Request);
            modelBuilder.Entity<RequestDocument>().Ignore(b => b.Document);
            modelBuilder.Entity<RequestDocument>().Property(b => b.Command).HasConversion(converter);
            base.OnModelCreating(modelBuilder);
        }

        public void OnCommandExecuted(object sender, CommandHandledEventArgs eventArgs)
        {
            // create document
            var requestDocument = RequestDocumentFactory.Create(eventArgs.Request, eventArgs.CommandHandled);
            
            switch (eventArgs.CommandHandled)
            {
                case Commands.Submit:
                    this.RequestDocuments.Add(requestDocument) ;
                    this.SaveChanges();
                    break;
                
                default:
                    break;
            }
        }
    }
}