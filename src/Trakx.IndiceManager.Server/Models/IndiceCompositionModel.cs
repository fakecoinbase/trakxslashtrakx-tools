using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Indice;
using Trakx.Persistence.DAO;

namespace Trakx.IndiceManager.Server.Models
{
    public class IndiceCompositionModel
    {
        // Non-nullable field is uninitialized. Consider declaring as nullable.
        // The empty constructor is for serialisation only.
        #pragma warning disable CS8618
        public IndiceCompositionModel() { }
        #pragma warning restore CS8618

        public IndiceCompositionModel(IIndiceComposition indiceComposition)
        {
            Address = indiceComposition.Address;
            Symbol = indiceComposition.Symbol;
            Version = indiceComposition.Version;
            CreationDate = indiceComposition.CreationDate;
            IndiceDetail = new IndiceDetailModel(indiceComposition.IndiceDefinition);
            Components = indiceComposition.ComponentQuantities
                .Select(q => new ComponentDetailModel(q))
                .ToList();
        }
        public IndiceDetailModel IndiceDetail { get; set; }

        public string Address { get; set; }

        public string Symbol { get; set; }

        public DateTime CreationDate { get; set; }

        public List<ComponentDetailModel> Components { get; set; }

        public uint Version { get; set; }

        public IIndiceComposition FromModelToIndiceComposition(IIndiceDefinition? indiceDefinition)
        {
            var indice= indiceDefinition?? new IndiceDefinitionDao(IndiceDetail.Symbol, IndiceDetail.Name,
                IndiceDetail.Description, IndiceDetail.NaturalUnit, IndiceDetail.Address, IndiceDetail.CreationDate);

            indice = string.IsNullOrWhiteSpace(indice.Address)
                ? new IndiceDefinitionDao(IndiceDetail.Symbol, IndiceDetail.Name,
                    IndiceDetail.Description, IndiceDetail.NaturalUnit, IndiceDetail.Address, IndiceDetail.CreationDate)
                : indice;

            var indiceCompositionDao= new IndiceCompositionDao((IndiceDefinitionDao)indice,Version,CreationDate,Address,Symbol);

            var componentQuantities = Components.Select(c=>new ComponentQuantityDao(indiceCompositionDao,
                new ComponentDefinitionDao(c.Address,c.Name,c.Symbol,c.CoinGeckoId,c.Decimals), Convert.ToUInt16(c.Quantity))).ToList<ComponentQuantityDao>();
            
            indiceCompositionDao.ComponentQuantityDaos = componentQuantities;

            return indiceCompositionDao;
        }
    }
}
