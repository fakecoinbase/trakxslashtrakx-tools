using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Persistence.DAO
{
    public class IndiceCompositionDao : IIndiceComposition
    {
        // Non-nullable field is uninitialized. Consider declaring as nullable.
        // This constructor is for serialisation only
        #pragma warning disable CS8618
        public IndiceCompositionDao(string address, string symbol)
        {
            Address = address;
            Symbol = symbol;
        }
        #pragma warning restore CS8618

        public IndiceCompositionDao(IndiceDefinitionDao indiceDefinition, 
            uint version, DateTime creationDate, string address, string symbol)
        {
            Id = $"{indiceDefinition.Symbol}|{version}";
            IndiceDefinitionDao = indiceDefinition;
            Version = version;
            CreationDate = creationDate;
            Address = address;
            Symbol = indiceDefinition.GetCompositionSymbol(creationDate);
            ComponentQuantityDaos = new List<ComponentQuantityDao>();
            IndiceValuationDaos = new List<IndiceValuationDao>();
        }

        public IndiceCompositionDao(IIndiceComposition composition)
        {
            IndiceDefinitionDao = new IndiceDefinitionDao(composition.IndiceDefinition.Symbol, composition.IndiceDefinition.Name, composition.IndiceDefinition.Description, composition.IndiceDefinition.NaturalUnit, composition.IndiceDefinition.Address, composition.IndiceDefinition.CreationDate);
            Version = composition.Version;
            CreationDate = composition.CreationDate;
            Address = composition.Address;
            Symbol = IndiceDefinition.GetCompositionSymbol(CreationDate);
            Id = $"{IndiceDefinition.Symbol}|{Version}";
            
            ComponentQuantityDaos = composition.ComponentQuantities.Select(c => new ComponentQuantityDao(this,
                new ComponentDefinitionDao(c.ComponentDefinition.Address, c.ComponentDefinition.Name, c.ComponentDefinition.Symbol, c.ComponentDefinition.CoinGeckoId, c.ComponentDefinition.Decimals), Convert.ToUInt64(c.Quantity.DescaleComponentQuantity(c.ComponentDefinition.Decimals, IndiceDefinitionDao.NaturalUnit)))).ToList<ComponentQuantityDao>();
            IndiceValuationDaos=new List<IndiceValuationDao>();
        }

        public List<ComponentQuantityDao> ComponentQuantityDaos { get; set; }

        #region Implementation of IIndiceComposition
        /// <summary>
        /// Unique identifier generated and used as a primary key on the database object.
        /// </summary>
        [Key]
        public string Id { get; private set; }

        /// <inheritdoc />
        public string Address { get; set; }

        /// <inheritdoc />
        public string Symbol { get; set; }

        /// <inheritdoc />
        [NotMapped]
        public IIndiceDefinition IndiceDefinition => IndiceDefinitionDao;

        public IndiceDefinitionDao IndiceDefinitionDao { get; set; }

        public List<IndiceValuationDao> IndiceValuationDaos { get; set; }

        /// <inheritdoc />
        [NotMapped]
        public List<IComponentQuantity> ComponentQuantities => 
            ComponentQuantityDaos.Cast<IComponentQuantity>().ToList();

        /// <inheritdoc />
        public uint Version { get; set; }

        /// <inheritdoc />
        public DateTime CreationDate { get; set; }

        #endregion
    }
}
