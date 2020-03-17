using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Trakx.Data.Persistence;
using Trakx.Data.Persistence.Initialisation;

namespace Trakx.Data.Tests.Unit.Models
{
    public sealed class InMemoryIndexRepositoryContext : IndexRepositoryContext
    {
        public InMemoryIndexRepositoryContext(IMapper mapper) : base(
            new DbContextOptionsBuilder<IndexRepositoryContext>()
            .UseInMemoryDatabase(databaseName: "IndexRepository")
            .Options)
        {
            DatabaseInitialiser.AddKnownIndexes(this, mapper);
        }
    }
}
