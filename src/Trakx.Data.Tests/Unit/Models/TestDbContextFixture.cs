using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Market.Server.Models;
using Trakx.Data.Persistence;
using Xunit;

namespace Trakx.Data.Tests.Unit.Models
{
    /// <summary>
    /// Use this context to get a simplified set of data aimed at testing.
    /// </summary>
    public sealed class TestDbContextFixture : IDisposable
    {
        public IndexRepositoryContext Context { get; private set; }
        
        public TestDbContextFixture()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMappings();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            Context = new TestIndexRepositoryContext(mapper);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Context?.Dispose();
        }

        #endregion
    }

    [CollectionDefinition(nameof(TestDbContextCollection))]
    public sealed class TestDbContextCollection : ICollectionFixture<TestDbContextFixture>
    {
        //nothing there 🤐
    }
}
