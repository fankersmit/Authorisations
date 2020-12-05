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

        // methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organisation>().HasKey(x => x.Id);
            modelBuilder.Entity<Contract>().HasKey(x => x.ContractID);
            modelBuilder.Entity<AccountRequest>().HasKey(x => x.Id);
            modelBuilder.Entity<Product>().HasKey(x => x.ProductId);
            modelBuilder.Entity<Person>().HasKey(x => x.PersonId);
            base.OnModelCreating(modelBuilder);
        }

        public void CommandExecuted(object sender, CommandHandledEventArgs eventArgs)
        {
            switch (eventArgs.CommandHandled)
            {
                case Commands.Submit:
                    var entity = eventArgs.Request;
                    AccountRequests.Add(entity as AccountRequest);
                    SaveChanges();
                    break;
                
                default:
                    break;
            }
        }
    }
}