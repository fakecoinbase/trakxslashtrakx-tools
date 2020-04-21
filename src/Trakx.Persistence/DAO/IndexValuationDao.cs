using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Trakx.Common.Interfaces.Index;

namespace Trakx.Persistence.DAO
{
    public class IndexValuationDao : IIndexValuation
    {
        // Non-nullable field is uninitialized. Consider declaring as nullable.
        // This constructor is for serialisation only
        #pragma warning disable CS8618
        public IndexValuationDao() {}
        #pragma warning restore CS8618

        public IndexValuationDao(List<ComponentValuationDao> componentValuations)
        {
            NetAssetValue = componentValuations.Sum(v => v.Value);
            var quoteCurrency = componentValuations.First().QuoteCurrency;
            QuoteCurrency = quoteCurrency;
            var timeStamp = componentValuations.Max(c => c.TimeStamp);
            TimeStamp = timeStamp;
            ComponentValuationDaos = componentValuations;
            var indexComposition = componentValuations.First().ComponentQuantityDao.IndexCompositionDao;
            IndexCompositionDao = indexComposition;

            componentValuations.ForEach(v => v.SetWeightFromTotalValue(NetAssetValue));

            Id = $"{indexComposition.Id}|{quoteCurrency}|{timeStamp:yyMMddHHmmssff}";
        }

        /// <summary>
        /// Unique identifier generated and used as a primary key on the database object.
        /// </summary>
        public string Id { get; set; }

        /// <inheritdoc />
        public List<IComponentValuation> ComponentValuations => 
            ComponentValuationDaos.Cast<IComponentValuation>().ToList();

        /// <inheritdoc />
        public DateTime TimeStamp { get; set; }

        /// <inheritdoc />
        public string QuoteCurrency { get; set; }

        /// <inheritdoc />
        [Column(TypeName = "decimal(38, 18)")]
        public decimal NetAssetValue { get; set; }

        /// <inheritdoc />
        public IIndexComposition IndexComposition => IndexCompositionDao;

        public IndexCompositionDao IndexCompositionDao { get; set; }
        public List<ComponentValuationDao> ComponentValuationDaos { get; set; }


    }
}