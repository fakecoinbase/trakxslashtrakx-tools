using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trakx.MarketData.Feeds.Common.StaticData
{
    public static class TrackerSymbols
    {
        [SingleName] public static string Btc = "BTC";
        [SingleName] public static string Eth = "ETH";
        [Basket] public static string MarketCap = "MC";

        private static IList<string> _allSymbols;

        public static IList<string> AllSymbols => _allSymbols
                                                  ?? (_allSymbols = typeof(TrackerSymbols)
                                                          .GetFields(BindingFlags.Public | BindingFlags.Static)
                                                          .Where(f => f.FieldType == typeof(string))
                                                          .Select(f => (string)f.GetValue(null)).ToList());

        private static IList<string> _allSingleNameSymbols;
        public static IList<string> AllSingleNameSymbols => _allSingleNameSymbols
                                                            ?? (_allSingleNameSymbols = typeof(TrackerSymbols)
                                                                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                                                                    .Where(f => f.FieldType == typeof(string) && f.GetCustomAttributes<SingleNameAttribute>().Any())
                                                                    .Select(f => (string)f.GetValue(null)).ToList());

        [AttributeUsage(AttributeTargets.Field)] private class SingleNameAttribute : Attribute { }
        [AttributeUsage(AttributeTargets.Field)] private class BasketAttribute : Attribute { }
    }
}