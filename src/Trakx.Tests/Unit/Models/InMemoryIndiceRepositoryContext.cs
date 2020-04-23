using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Trakx.Persistence;
using Trakx.Persistence.Initialisation;

namespace Trakx.Tests.Unit.Models
{
    public sealed class InMemoryIndiceRepositoryContext : IndiceRepositoryContext
    {
        public InMemoryIndiceRepositoryContext(IMapper mapper) : base(
            new DbContextOptionsBuilder<IndiceRepositoryContext>()
            .UseInMemoryDatabase(databaseName: "IndiceRepository")
            .Options)
        {
            DatabaseInitialiser.AddKnownIndicees(this, mapper);
        }
    }
}
