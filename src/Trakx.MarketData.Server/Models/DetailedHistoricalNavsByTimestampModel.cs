using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Pricing;
using Trakx.Common.Serialisation.Converters;

namespace Trakx.MarketData.Server.Models
{
    /// <summary>
    /// Historical Net Asset Values of a given index, indexed by their <see cref="IndiceValuationModel.TimeStamp"/>.
    /// </summary>
    public class DetailedHistoricalNavsByTimestampModel
    {
        #nullable disable
        /// <inheritdoc />
        public DetailedHistoricalNavsByTimestampModel() { }
        #nullable restore

        /// <inheritdoc />
        public DetailedHistoricalNavsByTimestampModel(
            string symbol,
            DateTime startTime,
            Period period,
            IEnumerable<IIndiceValuation> indexValuations,
            DateTime? endTime)
        {
            Symbol = symbol;
            StartTime = startTime;
            Period = period;
            EndTime = endTime;
            ValuationsByTimeStamp = indexValuations.ToDictionary(v => v.TimeStamp, v => new IndiceValuationModel(v));
        }

        /// <summary>
        /// The symbol (index or composition) for which the valuations were calculated.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// A history of the valuations of the Index, indexed by timestamp.
        /// </summary>
        [JsonConverter(typeof(JsonNonStringKeyDictionaryConverter<DateTime, IndiceValuationModel>))]
        public IDictionary<DateTime, IndiceValuationModel> ValuationsByTimeStamp { get; set; }

        /// <summary>
        /// Earliest time for which the valuations have been requested.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Latest time for which the valuations have been requested.
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Period with which the valuations have been calculated.
        /// </summary>
        public Period Period { get; set; }
    }
}