using Microsoft.EntityFrameworkCore;

namespace Trakx.Persistence.Tests.Model
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