using Microsoft.EntityFrameworkCore;
using Trakx.Data.Models.Index;
using Trakx.Data.Models.Initialisation;

namespace Trakx.Data.Market.Tests.Unit.Models
{
    public sealed class TestIndexRepositoryContext : IndexRepositoryContext
    {
        public TestIndexRepositoryContext() : base(new DbContextOptionsBuilder<IndexRepositoryContext>()
            .UseInMemoryDatabase(databaseName: "IndexRepository")
            .Options)
        {
            var indexes = DatabaseInitialiser.GetKnownIndexes();

            AddRange(indexes);
            SaveChanges();
        }
    }
}
