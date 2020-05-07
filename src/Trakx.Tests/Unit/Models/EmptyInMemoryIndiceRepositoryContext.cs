using Microsoft.EntityFrameworkCore;
using Trakx.Persistence;

namespace Trakx.Tests.Unit.Models
{
    public class EmptyInMemoryIndiceRepositoryContext : IndiceRepositoryContext
    {
        public EmptyInMemoryIndiceRepositoryContext() : base(
            new DbContextOptionsBuilder<IndiceRepositoryContext>()
                .UseInMemoryDatabase(databaseName: "EmptyIndiceRepository")
                .Options)
        {
        }
    }
}