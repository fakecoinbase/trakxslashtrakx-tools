using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Trakx.MarketData.Server.Models;
using Xunit;

namespace Trakx.Tests.Unit.Models
{
    public sealed class DbContextFixture : IDisposable
    {
        public InMemoryIndexRepositoryContext Context { get; private set; }
        

        public DbContextFixture()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMappings();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            Context = new InMemoryIndexRepositoryContext(mapper);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Context?.Dispose();
        }

        #endregion
    }

    [CollectionDefinition(nameof(DbContextCollection))]
    public sealed class DbContextCollection : ICollectionFixture<DbContextFixture>
    {
        //nothing there 🤐
    }
}