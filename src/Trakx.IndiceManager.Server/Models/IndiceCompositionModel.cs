using System;
using System.Collections.Generic;
using System.Linq;
using Trakx.Common.Interfaces.Indice;

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
    }
}
