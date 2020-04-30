using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trakx.IndiceManager.Server.Models
{
    public class AccountBalanceModel
    {
        public AccountBalanceModel(string token, decimal balance)
        {
            Token = token;
            Balance = balance;
        }

        public string Token { get; set; }

        public decimal Balance { get; set; }
    }
}
