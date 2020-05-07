using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Trakx.MarketData.Server.Models;
using Xunit;

namespace Trakx.Tests.Unit.Models
{
    public sealed class SeededDbContextFixture : IDisposable
    {
        public SeededInMemoryIndiceRepositoryContext Context { get; private set; }
        
        public SeededDbContextFixture()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMappings();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            Context = new SeededInMemoryIndiceRepositoryContext(mapper);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Context?.Dispose();
        }

        #endregion
    }

    [CollectionDefinition(nameof(SeededDbContextCollection))]
    public sealed class SeededDbContextCollection : ICollectionFixture<SeededDbContextFixture>
    {
        //nothing there 🤐
    }
}