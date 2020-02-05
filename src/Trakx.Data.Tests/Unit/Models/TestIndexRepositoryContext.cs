using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Trakx.Data.Persistence;
using Trakx.Data.Persistence.Initialisation;

namespace Trakx.Data.Tests.Unit.Models
{
    public sealed class TestIndexRepositoryContext : IndexRepositoryContext
    {
        public TestIndexRepositoryContext(IMapper mapper) : base(
            new DbContextOptionsBuilder<IndexRepositoryContext>()
            .UseInMemoryDatabase(databaseName: "IndexRepository")
            .Options)
        {
            
            var components = DatabaseInitialiser.AddKnownIndexes(this, mapper);
        }

        #region Overrides of IndexRepositoryContext

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }

        #endregion
    }
}
