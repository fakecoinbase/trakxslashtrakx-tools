using System.Linq;
using Microsoft.EntityFrameworkCore;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public static class DbContextExtensions
    {
        public static IQueryable<IndexCompositionDao> IncludeAllLinkedEntities(this DbSet<IndexCompositionDao> indexCompositionDaos)
        {
            return indexCompositionDaos.Include(c => c.ComponentQuantityDaos)
                .ThenInclude(c => c.ComponentDefinitionDao)
                .Include(c => c.IndexDefinitionDao);
        }

        public static IQueryable<IndexValuationDao> IncludeAllLinkedEntities(
            this DbSet<IndexValuationDao> indexValuationsDaos)
        {
            return indexValuationsDaos.Include(i => i.ComponentValuationDaos)
                .ThenInclude(v => v.ComponentQuantityDao)
                .ThenInclude(q => q.ComponentDefinitionDao)
                .Include(i => i.IndexCompositionDao)
                .ThenInclude(c => c.ComponentQuantityDaos)
                .ThenInclude(q => q.ComponentDefinitionDao)
                .Include(i => i.IndexCompositionDao)
                .ThenInclude(c => c.IndexDefinitionDao);
        }
    }
}