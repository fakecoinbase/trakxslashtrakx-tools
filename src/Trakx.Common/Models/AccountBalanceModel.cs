using System;

namespace Trakx.Common.Models
{
    public class AccountBalanceModel
    {
        public AccountBalanceModel(string symbol,
            decimal balance,
            ulong nativeBalance, 
            string name, 
            string address, 
            DateTimeOffset lastUpDate)
        {
            Symbol = symbol;
            Balance = balance;
            NativeBalance = nativeBalance;
            Name = name;
            Address = address;
            LastUpDate = lastUpDate;
        }

        public string Symbol { get; set; }
        public ulong NativeBalance { get; set; }
        public decimal Balance { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTimeOffset LastUpDate { get; set; }
    }
}
