using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces.Indice;
using Trakx.Persistence;
using Trakx.Persistence.DAO;
using Trakx.Persistence.Initialisation;

namespace Trakx.Tests.Unit.Models
{
    /// <summary>
    /// Simplified fake database with predictable content, mostly aimed at testing IndiceDataProvider
    /// </summary>
    public sealed class TestIndiceRepositoryContext : IndiceRepositoryContext
    {
        private readonly IMapper _mapper;
        private readonly Dictionary<string, int> _componentCountPerIndice;
        private readonly DateTime _firstJan;

        public TestIndiceRepositoryContext(IMapper mapper) : base(
            new DbContextOptionsBuilder<IndiceRepositoryContext>()
                .UseInMemoryDatabase("IndiceRepository")
                .Options)
        {
            _mapper = mapper;
            _firstJan = new DateTime(2020, 1, 1);
            _componentCountPerIndice = new Dictionary<string, int>()
            {
                {"l1abc", 3},
                {"s3def", 20},
                {"l8ghi", 37},
                {"s13jkl", 73},
                {"l22mno", 201}
            };

            AddIndicesDefinitions();
            AddComponentDefinitions();
            CreateIndiceCompositions();
        }

        public void AddComponentDefinitions()
        {
            var i = 0;
            foreach (var pair in _componentCountPerIndice)
            {
                var indiceTicker = pair.Key.GetSectorTickerFromIndiceSymbol();
                var indiceNumber = i++;
                var components = Enumerable.Range(1, pair.Value)
                    .Select(j =>
                        new ComponentDefinitionDao(
                            $"0x{indiceNumber}000000000000000000000000000000000000{j:000}",
                            $"{indiceTicker} component {j:000}",
                            $"{indiceTicker}c{j:000}",
                            $"i{indiceTicker}-c{j:000}", (ushort)(18 - j%18))
                    );
                
                ComponentDefinitions.AddRange(components);
            }

            SaveChanges();
        }

        public void AddIndicesDefinitions()
        {
            var i = 0;
            foreach (var pair in _componentCountPerIndice)
            {
                i++;
                var indiceTicker = pair.Key.GetSectorTickerFromIndiceSymbol();
                var indice = new IndiceDefinitionDao(pair.Key, $"Indice {indiceTicker}", 
                    $"Description of {indiceTicker}", 10,
                    $"0x{i}000000000000000000000000000000000000000", _firstJan);
                
                IndiceDefinitions.Add(indice);
            }
            SaveChanges();
        }

        public void CreateIndiceCompositions()
        {
            var indiceBySymbols = IndiceDefinitions.ToDictionary(i => i.Symbol, i => i);
            var componentsBySymbols = ComponentDefinitions.ToDictionary(i => i.Symbol, i => i);

            var allCompositionData = _componentCountPerIndice.Select(pair =>
                {
                    var indiceTicker = pair.Key.GetSectorTickerFromIndiceSymbol();
                    var components = Enumerable.Range(1, pair.Value).Select(i =>
                            new DatabaseInitialiser.CompositionData(indiceBySymbols[pair.Key], _firstJan,
                                componentsBySymbols[$"{indiceTicker}c{i:000}"],
                                10 * i, 1 / (decimal) pair.Value))
                        .ToList();
                    return components;
                }).SelectMany(c => c).ToList();
             
            foreach (var indiceDefinition in allCompositionData.Select(c => c.IndiceDefinition).Distinct())
            {
                var compositionData = allCompositionData.Where(c => c.IndiceDefinition == indiceDefinition).ToList();
                DatabaseInitialiser.AddCompositionAndInitialValuations(
                    this, _mapper, _firstJan, 1, indiceDefinition, compositionData, CancellationToken.None)
                    .GetAwaiter().GetResult();
            }

        }

    }
}