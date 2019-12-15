using Microsoft.EntityFrameworkCore;


namespace Trakx.Data.Models.Index
{
    public class IndexRepositoryContext : DbContext
    {
        /// <inheritdoc />
        public IndexRepositoryContext(DbContextOptions options) 
            : base(options)
        {}

        public DbSet<IndexDefinition> IndexDefinitions { get; set; }
    }
}
