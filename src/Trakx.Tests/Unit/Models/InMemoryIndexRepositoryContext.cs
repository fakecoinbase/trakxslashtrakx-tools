using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Trakx.Persistence;
using Trakx.Persistence.Initialisation;

namespace Trakx.Tests.Unit.Models
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
