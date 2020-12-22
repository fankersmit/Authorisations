using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Requests.Domain;
using Requests.Shared.Domain;

namespace RequestsApp.Infrastructure
{
    
    public class RequestDbContext : DbContext
    {
        private readonly ILogger<RequestDbContext> _logger; 
        // ctors
        public RequestDbContext ( ILogger<RequestDbContext> logger, DbContextOptions<RequestDbContext>  options ): base (options)
        {
            _logger = logger;
        }

        // repo
        public DbSet<RequestDocument> RequestDocuments { get; set; }
        
        // methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new EnumToStringConverter<Commands>();
            modelBuilder.Entity<RequestDocument>().HasKey(o => new { o.ID, o.Version });
            modelBuilder.Entity<RequestDocument>().Ignore(b => b.Request);
            modelBuilder.Entity<RequestDocument>().Ignore(b => b.Document);
            modelBuilder.Entity<RequestDocument>().Property(b => b.Command).HasConversion(converter);
            base.OnModelCreating(modelBuilder);
        }

        public void OnCommandExecuted(object sender, CommandHandledEventArgs eventArgs)
        {
            // rerieve document from eventArgs 
            var requestDocument = RequestDocumentFactory.Create(eventArgs.Request, eventArgs.CommandHandled);
            var Id = requestDocument.ID;
            var version = requestDocument.Version;
            // store, if not already present
            if (DocumentIsAlreadyAdded(Id, version))
            {
                _logger.Log(LogLevel.Information,$"Skipped adding request again with ID:{Id} and version:{version}");
                return;
            }
        
            this.RequestDocuments.Add(requestDocument);
            this.SaveChanges();
        }

        private bool DocumentIsAlreadyAdded(Guid Id, int version)
        {
            var local = this.Set<RequestDocument>()
                .Local
                .FirstOrDefault(entry => entry.ID.Equals(Id) && entry.Version.Equals(version));
            return (local != null);
        }
    }
}