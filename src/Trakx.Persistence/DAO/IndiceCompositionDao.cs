﻿using System;
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
        // This constructor is for serialisation only
        #nullable disable
        public IndiceCompositionDao() { }
        #nullable restore

        public IndiceCompositionDao(IndiceDefinitionDao indiceDefinition, 
            uint version, DateTime creationDate, string address)
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
            IndiceDefinitionDao = new IndiceDefinitionDao(composition.IndiceDefinition.Symbol,
                composition.IndiceDefinition.Name, composition.IndiceDefinition.Description,
                composition.IndiceDefinition.NaturalUnit, composition.IndiceDefinition.Address,
                composition.IndiceDefinition.CreationDate);
            Version = composition.Version;
            CreationDate = composition.CreationDate;
            Address = composition.Address;
            Symbol = composition.Symbol;
            Id = composition.GetCompositionId();

            ComponentQuantityDaos = composition.ComponentQuantities.Select(c => new ComponentQuantityDao(this,
                new ComponentDefinitionDao(c.ComponentDefinition.Address, c.ComponentDefinition.Name,
                    c.ComponentDefinition.Symbol, c.ComponentDefinition.CoinGeckoId, c.ComponentDefinition.Decimals),
                Convert.ToUInt64(c.Quantity.DescaleComponentQuantity(c.ComponentDefinition.Decimals,
                    IndiceDefinitionDao.NaturalUnit)))).ToList();
            IndiceValuationDaos=new List<IndiceValuationDao>();
        }

        public List<ComponentQuantityDao> ComponentQuantityDaos { get; set; }

        #region Implementation of IIndiceComposition
        /// <summary>
        /// Unique identifier generated and used as a primary key on the database object.
        /// </summary>
        [Key]
        public string Id { get; private set;  }

        /// <inheritdoc />
        public string? Address { get; set; }

        /// <inheritdoc />
        public string Symbol { get; set; }

        /// <inheritdoc />
        [NotMapped]
        public IIndiceDefinition IndiceDefinition => IndiceDefinitionDao;

        [Required]
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
