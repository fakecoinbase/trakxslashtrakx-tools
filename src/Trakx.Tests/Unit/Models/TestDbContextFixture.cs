using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Trakx.MarketData.Server.Models;
using Trakx.Persistence;
using Xunit;

namespace Trakx.Tests.Unit.Models
{
    /// <summary>
    /// Use this context to get a simplified set of data aimed at testing.
    /// </summary>
    public sealed class TestDbContextFixture : IDisposable
    {
        public IndiceRepositoryContext Context { get; private set; }
        
        public TestDbContextFixture()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMappings();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            Context = new TestIndiceRepositoryContext(mapper);
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
