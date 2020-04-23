using System.Linq;
using Microsoft.EntityFrameworkCore;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public static class DbContextExtensions
    {
        public static IQueryable<IndiceCompositionDao> IncludeAllLinkedEntities(this DbSet<IndiceCompositionDao> indiceCompositionDaos)
        {
            return indiceCompositionDaos.Include(c => c.ComponentQuantityDaos)
                .ThenInclude(c => c.ComponentDefinitionDao)
                .Include(c => c.IndiceDefinitionDao);
        }

        public static IQueryable<IndiceValuationDao> IncludeAllLinkedEntities(
            this DbSet<IndiceValuationDao> indiceValuationsDaos)
        {
            return indiceValuationsDaos.Include(i => i.ComponentValuationDaos)
                .ThenInclude(v => v.ComponentQuantityDao)
                .ThenInclude(q => q.ComponentDefinitionDao)
                .Include(i => i.IndiceCompositionDao)
                .ThenInclude(c => c.ComponentQuantityDaos)
                .ThenInclude(q => q.ComponentDefinitionDao)
                .Include(i => i.IndiceCompositionDao)
                .ThenInclude(c => c.IndiceDefinitionDao);
        }
    }
}