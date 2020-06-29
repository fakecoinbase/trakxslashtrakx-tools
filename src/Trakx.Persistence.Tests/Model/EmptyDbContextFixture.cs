using System;
using Xunit;

namespace Trakx.Persistence.Tests.Model
{
    public sealed class EmptyDbContextFixture : IDisposable
    {
        public EmptyInMemoryIndiceRepositoryContext Context { get; private set; }
        

        public EmptyDbContextFixture()
        {
            Context = new EmptyInMemoryIndiceRepositoryContext();
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Context?.Dispose();
        }

        #endregion
    }

    [CollectionDefinition(nameof(EmptyDbContextCollection))]
    public class EmptyDbContextCollection : ICollectionFixture<EmptyDbContextFixture>
    {
        //nothing there 🤐
    }
}