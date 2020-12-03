using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RequestsApp.Infrastructure;


namespace Tests.Helpers
{
    public class SqliteInMemoryFixture : IDisposable
    {
        // fields
        private readonly DbConnection _connection; 
        private readonly RequestDbContext _context;

        // properties
        public RequestDbContext Context => _context;

        // ctors
        public SqliteInMemoryFixture()
        {
           _connection = CreateInMemoryDatabaseConnection();
           var contextOptions  = new DbContextOptionsBuilder<RequestDbContext>()
                .UseSqlite(_connection)
                .Options;

           _context = CreateInMemoryContext(contextOptions);
        }

        // methods
        private RequestDbContext CreateInMemoryContext( DbContextOptions<RequestDbContext> contextOptions)
        {
           var  context  =  new RequestDbContext(contextOptions);
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