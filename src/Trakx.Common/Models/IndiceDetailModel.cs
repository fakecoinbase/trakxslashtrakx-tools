using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Models
{
    public class IndiceDetailModel
    {
        // Non-nullable field is uninitialized. Consider declaring as nullable.
        // The empty constructor is for serialisation only.
        #pragma warning disable CS8618
        public IndiceDetailModel() { }
        #pragma warning restore CS8618

        public IndiceDetailModel(IIndiceDefinition indice)
        {
            Symbol = indice.Symbol;
            CreationDate = indice.CreationDate;
            Name = indice.Name;
            Address = indice.Address;
            Description = indice.Description;
            NaturalUnit = indice.NaturalUnit;
            IndiceState = string.IsNullOrWhiteSpace(indice.Address) 
                ? "Saved" 
                : "Published";
        }
        [Required]
        public string? Symbol { get; set; }

        [Required]
        public string? Name { get; set; }

        public DateTime? CreationDate { get; set; }

        public string? IndiceState { get; set; }

        public string? Address { get; set; }

        public string? Description { get; set; }

        public ushort NaturalUnit { get; set; }

        public ICollection<IndiceCompositionModel>? IndiceCompositions { get; set; }
    }
}
