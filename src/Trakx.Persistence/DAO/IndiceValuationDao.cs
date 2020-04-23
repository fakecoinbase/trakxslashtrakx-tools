using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Persistence.DAO
{
    public class IndiceValuationDao : IIndiceValuation
    {
        // Non-nullable field is uninitialized. Consider declaring as nullable.
        // This constructor is for serialisation only
        #pragma warning disable CS8618
        public IndiceValuationDao() {}
        #pragma warning restore CS8618

        public IndiceValuationDao(List<ComponentValuationDao> componentValuations)
        {
            NetAssetValue = componentValuations.Sum(v => v.Value);
            var quoteCurrency = componentValuations.First().QuoteCurrency;
            QuoteCurrency = quoteCurrency;
            var timeStamp = componentValuations.Max(c => c.TimeStamp);
            TimeStamp = timeStamp;
            ComponentValuationDaos = componentValuations;
            var indiceComposition = componentValuations.First().ComponentQuantityDao.IndiceCompositionDao;
            IndiceCompositionDao = indiceComposition;

            componentValuations.ForEach(v => v.SetWeightFromTotalValue(NetAssetValue));

            Id = $"{indiceComposition.Id}|{quoteCurrency}|{timeStamp:yyMMddHHmmssff}";
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
        public IIndiceComposition IndiceComposition => IndiceCompositionDao;

        public IndiceCompositionDao IndiceCompositionDao { get; set; }
        public List<ComponentValuationDao> ComponentValuationDaos { get; set; }


    }
}