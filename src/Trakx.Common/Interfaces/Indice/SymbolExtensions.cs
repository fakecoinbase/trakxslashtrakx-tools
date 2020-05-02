using System;
using System.Text.RegularExpressions;

namespace Trakx.Common.Interfaces.Indice
{
    public static class SymbolExtensions
    {
        public static readonly Regex IndiceSymbolRegex = new Regex(
            @"^(?=^.{4,10}$)(?<longShort>l|s)(?<leverage>[0-9]+)(?<sectorTicker>[a-zA-z]+[\w]*)$");
        public static readonly Regex CompositionSymbolRegex = new Regex(
            @"^(?=^.{8,14}$)(?<longShort>l|s)(?<leverage>[0-9]+)(?<sectorTicker>[a-zA-z]+[\w]*)(?<dateTicker>[0-9][0-9](0[1-9]|1[0-2]))$");

        public static bool IsIndiceSymbol(this string candidateSymbol)
        {
            return !IsCompositionSymbol(candidateSymbol) && IndiceSymbolRegex.IsMatch(candidateSymbol);
        }

        public static bool IsCompositionSymbol(this string candidateSymbol)
        {
            return CompositionSymbolRegex.IsMatch(candidateSymbol);
        }

        public static DateTime GetDateFromCompositionSymbol(this string compositionSymbol)
        {
            var matches = CompositionSymbolRegex.Match(compositionSymbol);
            var dateTicker = matches.Groups["dateTicker"].Value;
            var year = int.Parse(dateTicker.Substring(0,2));
            var month = int.Parse(dateTicker.Substring(2,2));
            var date = new DateTime(year, month, 1);
            return date;
        }

        public static string GetSectorTickerFromIndiceSymbol(this string indiceSymbol)
        {
            var matches = IndiceSymbolRegex.Match(indiceSymbol);
            var sectorTicker = matches.Groups["sectorTicker"].Value;
            return sectorTicker;
        }
    }
}