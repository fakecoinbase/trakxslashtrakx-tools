using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Trakx.Persistence;
using Trakx.Persistence.Initialisation;

namespace Trakx.Tests.Unit.Models
{
    public sealed class SeededInMemoryIndiceRepositoryContext : IndiceRepositoryContext
    {
        public SeededInMemoryIndiceRepositoryContext(IMapper mapper) : base(
            new DbContextOptionsBuilder<IndiceRepositoryContext>()
                .UseInMemoryDatabase(databaseName: "SeededIndiceRepository")
                .Options)

        {
            DatabaseInitialiser.AddKnownIndicees(this, mapper);
        }
    }
}
