using Microsoft.EntityFrameworkCore;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public partial class IndiceRepositoryContext : DbContext
    {
        /// <inheritdoc />
        public IndiceRepositoryContext(DbContextOptions options) 
            : base(options)
        {}

        public IndiceRepositoryContext(): base(new DbContextOptionsBuilder<IndiceRepositoryContext>()
            .UseSqlServer("Server=localhost,1533;Database=IndiceRepository;Trusted_Connection=False;MultipleActiveResultSets=true;User Id=SA;Password=oh_no_this-needs_toChange4Real")
            .Options)
        {}

        public DbSet<ComponentQuantityDao> ComponentQuantities { get; set; }
        public DbSet<ComponentDefinitionDao> ComponentDefinitions { get; set; }
        public DbSet<ComponentValuationDao> ComponentValuations { get; set; }
        public DbSet<IndiceSupplyTransactionDao> IndiceSupplyTransactions { get; set; }
        public DbSet<IndiceDefinitionDao> IndiceDefinitions { get; set; }
        public DbSet<IndiceCompositionDao> IndiceCompositions { get; set; }
        public DbSet<IndiceValuationDao> IndiceValuations { get; set; }
        public DbSet<WrappingTransactionDao> WrappingTransactions { get; set; }
        public DbSet<UserAddressDao> UserAddresses { get; set; }
        
        #region Overrides of DbContext

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
        }

        #endregion
    }
}
