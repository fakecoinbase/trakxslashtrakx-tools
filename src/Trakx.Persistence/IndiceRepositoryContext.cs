using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Trakx.Persistence.DAO;
using Trakx.Common.Interfaces;

namespace Trakx.Persistence
{
    public partial class IndiceRepositoryContext : DbContext
    {
        public IndiceRepositoryContext() : base(new DbContextOptionsBuilder<IndiceRepositoryContext>()
            .UseSqlServer("")
            .Options)
        { }

        /// <inheritdoc />
        public IndiceRepositoryContext(DbContextOptions options)
            : base(options)
        {
            ChangeTracker.Tracked += OnEntityTracked;
            ChangeTracker.StateChanged += OnEntityStateChanged;
        }

        void OnEntityTracked(object? sender, EntityTrackedEventArgs e)
        {
            if (!e.FromQuery && e.Entry.State == EntityState.Added
                             && e.Entry.Entity is IHasCreatedLastModified entity)
                entity.Created = DateTime.UtcNow;
        }

        void OnEntityStateChanged(object? sender, EntityStateChangedEventArgs e)
        {
            if (e.NewState == EntityState.Modified
                && e.Entry.Entity is IHasCreatedLastModified entity)
                entity.LastModified = DateTime.UtcNow;
        }

        public DbSet<ComponentQuantityDao> ComponentQuantities { get; set; }
        public DbSet<ComponentDefinitionDao> ComponentDefinitions { get; set; }
        public DbSet<ComponentValuationDao> ComponentValuations { get; set; }
        public DbSet<IndiceSupplyTransactionDao> IndiceSupplyTransactions { get; set; }
        public DbSet<IndiceDefinitionDao> IndiceDefinitions { get; set; }
        public DbSet<IndiceCompositionDao> IndiceCompositions { get; set; }
        public DbSet<IndiceValuationDao> IndiceValuations { get; set; }
        public DbSet<WrappingTransactionDao> WrappingTransactions { get; set; }
        public DbSet<UserDao> Users { get; set; }
        public DbSet<DepositorAddressDao> DepositorAddresses { get; set; }

        #region Overrides of DbContext

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}
