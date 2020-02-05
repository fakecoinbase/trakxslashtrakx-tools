using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Persistence.DAO
{
    public class IndexValuationDao : IIndexValuation
    {
        public IndexValuationDao() {}

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