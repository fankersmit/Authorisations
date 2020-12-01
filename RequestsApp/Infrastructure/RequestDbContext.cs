using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Requests.Domain;


namespace RequestsApp.Infrastructure
{
    
    public class RequestDbContext : DbContext
    {
        public RequestDbContext ( DbContextOptions<RequestDbContext>  options ): base (options)
        {
        }

        public DbSet<AccountRequest> AccountRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organisation>().HasKey(x => x.Id);
            modelBuilder.Entity<Contract>().HasKey(x => x.ContractID);
            modelBuilder.Entity<AccountRequest>().HasKey(x => x.Id);
            modelBuilder.Entity<Product>().HasKey(x => x.ProductId);
            modelBuilder.Entity<Person>().HasKey(x => x.PersonId);
            base.OnModelCreating(modelBuilder);
        }
    }
}