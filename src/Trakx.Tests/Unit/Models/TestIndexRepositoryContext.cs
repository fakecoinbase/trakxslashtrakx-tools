using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces.Index;
using Trakx.Persistence;
using Trakx.Persistence.DAO;
using Trakx.Persistence.Initialisation;

namespace Trakx.Tests.Unit.Models
{
    /// <summary>
    /// Simplified fake database with predictable content, mostly aimed at testing IndexDataProvider
    /// </summary>
    public sealed class TestIndexRepositoryContext : IndexRepositoryContext
    {
        private readonly IMapper _mapper;
        private readonly Dictionary<string, int> _componentCountPerIndex;
        private readonly DateTime _firstJan;

        public TestIndexRepositoryContext(IMapper mapper) : base(
            new DbContextOptionsBuilder<IndexRepositoryContext>()
                .UseInMemoryDatabase("IndexRepository")
                .Options)
        {
            _mapper = mapper;
            _firstJan = new DateTime(2020, 1, 1);
            _componentCountPerIndex = new Dictionary<string, int>()
            {
                {"l1abc", 3},
                {"s3def", 20},
                {"l8ghi", 37},
                {"s13jkl", 73},
                {"l22mno", 201}
            };

            AddIndicesDefinitions();
            AddComponentDefinitions();
            CreateIndexCompositions();
        }

        public void AddComponentDefinitions()
        {
            var i = 0;
            foreach (var pair in _componentCountPerIndex)
            {
                var indexTicker = pair.Key.GetSectorTickerFromIndexSymbol();
                var indexNumber = i++;
                var components = Enumerable.Range(1, pair.Value)
                    .Select(j =>
                        new ComponentDefinitionDao(
                            $"0x{indexNumber}000000000000000000000000000000000000{j:000}",
                            $"{indexTicker} component {j:000}",
                            $"{indexTicker}c{j:000}",
                            $"i{indexTicker}-c{j:000}", (ushort)(18 - j%18))
                    );
                
                ComponentDefinitions.AddRange(components);
            }

            SaveChanges();
        }

        public void AddIndicesDefinitions()
        {
            var i = 0;
            foreach (var pair in _componentCountPerIndex)
            {
                i++;
                var indexTicker = pair.Key.GetSectorTickerFromIndexSymbol();
                var index = new IndexDefinitionDao(pair.Key, $"Index {indexTicker}", 
                    $"Description of {indexTicker}", 10,
                    $"0x{i}000000000000000000000000000000000000000", _firstJan);
                
                IndexDefinitions.Add(index);
            }
            SaveChanges();
        }

        public void CreateIndexCompositions()
        {
            var indexBySymbols = IndexDefinitions.ToDictionary(i => i.Symbol, i => i);
            var componentsBySymbols = ComponentDefinitions.ToDictionary(i => i.Symbol, i => i);

            var allCompositionData = _componentCountPerIndex.Select(pair =>
                {
                    var indexTicker = pair.Key.GetSectorTickerFromIndexSymbol();
                    var components = Enumerable.Range(1, pair.Value).Select(i =>
                            new DatabaseInitialiser.CompositionData(indexBySymbols[pair.Key], _firstJan,
                                componentsBySymbols[$"{indexTicker}c{i:000}"],
                                10 * i, 1 / (decimal) pair.Value))
                        .ToList();
                    return components;
                }).SelectMany(c => c).ToList();
             
            foreach (var indexDefinition in allCompositionData.Select(c => c.IndexDefinition).Distinct())
            {
                var compositionData = allCompositionData.Where(c => c.IndexDefinition == indexDefinition).ToList();
                DatabaseInitialiser.AddCompositionAndInitialValuations(
                    this, _mapper, _firstJan, 1, indexDefinition, compositionData, CancellationToken.None)
                    .GetAwaiter().GetResult();
            }

        }

    }
}