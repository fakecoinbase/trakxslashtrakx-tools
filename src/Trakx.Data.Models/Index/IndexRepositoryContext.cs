using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;


namespace Trakx.Data.Models.Index
{
    public class IndexRepositoryContext : DbContext
    {
        /// <inheritdoc />
        public IndexRepositoryContext(DbContextOptions options) 
            : base(options)
        {}

        public DbSet<IndexDefinition> IndexDefinitions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<IndexDefinition>()
            //    .HasKey(e => e.Symbol);

            //modelBuilder.Entity<ComponentDefinition>()
            //    .HasKey(c => c.ComponentDefinitionId);

            //modelBuilder.Entity<ComponentValuation>()
            //    .HasKey(c => c.C);
        }
    }
}
