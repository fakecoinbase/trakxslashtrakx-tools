using Microsoft.EntityFrameworkCore;
using Trakx.Data.Persistence.DAO;

namespace Trakx.Data.Persistence
{
    public class IndexRepositoryContext : DbContext
    {
        /// <inheritdoc />
        public IndexRepositoryContext(DbContextOptions options) 
            : base(options)
        {}

        public IndexRepositoryContext(): base(new DbContextOptionsBuilder<IndexRepositoryContext>()
            .UseSqlServer("Server=localhost,1533;Database=IndexRepository;Trusted_Connection=False;MultipleActiveResultSets=true;User Id=SA;Password=oh_no_this-needs_toChange4Real")
            .Options)
        {}

        public DbSet<ComponentQuantityDao> ComponentQuantities { get; set; }
        public DbSet<ComponentWeightDao> ComponentWeights { get; set; }
        public DbSet<ComponentDefinitionDao> ComponentDefinitions { get; set; }
        public DbSet<ComponentValuationDao> ComponentValuations { get; set; }
        
        public DbSet<IndexDefinitionDao> IndexDefinitions { get; set; }
        public DbSet<IndexCompositionDao> IndexCompositions { get; set; }
        public DbSet<IndexValuationDao> IndexValuations { get; set; }

        #region Overrides of DbContext

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}
