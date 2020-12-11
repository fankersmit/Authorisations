using Microsoft.EntityFrameworkCore;
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
        public DbSet<AccountRequest> AccountRequests { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Product> Products{ get; set; }
        public DbSet<Product> Organisation{ get; set; }
        
        
        // methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountRequest>().HasKey(x => x.ID);
            modelBuilder.Entity<Organisation>().HasKey(x => x.ID);
            modelBuilder.Entity<Contract>().HasKey(x => x.ID);
            modelBuilder.Entity<Product>().HasKey(x => x.ID);
            modelBuilder.Entity<Person>().HasKey(x => x.ID);
            base.OnModelCreating(modelBuilder);
        }

        public void OnCommandExecuted(object sender, CommandHandledEventArgs eventArgs)
        {
            var entity = eventArgs.Request as AccountRequest;
            
            switch (eventArgs.CommandHandled)
            {
                case Commands.Submit:
                    this.AccountRequests.Add(entity) ;
                    this.SaveChanges();
                    break;
                
                default:
                    break;
            }
        }
    }
}