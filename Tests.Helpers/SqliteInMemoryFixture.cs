using System;
using System.Data.Common;
using Authorisations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using RequestsApp.Infrastructure;


namespace Tests.Helpers
{
    public class SqliteInMemoryFixture : IDisposable
    {
        // fields
        private readonly DbConnection _connection; 
        private readonly RequestDbContext _context;
        private readonly ILogger<RequestDbContext> _logger;

        // properties
        public RequestDbContext Context => _context;

        // ctors
        public SqliteInMemoryFixture()
        {
           _connection = CreateInMemoryDatabaseConnection();
           var contextOptions  = new DbContextOptionsBuilder<RequestDbContext>()
                .UseSqlite(_connection)
                .Options;

           ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
           _logger = loggerFactory.CreateLogger<RequestDbContext>();
           
           _context = CreateInMemoryContext(_logger, contextOptions);
        }

        // methods
        private RequestDbContext CreateInMemoryContext( ILogger<RequestDbContext> logger, DbContextOptions<RequestDbContext> contextOptions)
        {
            var  context  =  new RequestDbContext(logger, contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }
        
        private static DbConnection CreateInMemoryDatabaseConnection()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }
        
        // helper methods
        private void ReleaseUnmanagedResources()
        {
            _connection.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~SqliteInMemoryFixture()
        {
            ReleaseUnmanagedResources();
        }
        
       
    }
}