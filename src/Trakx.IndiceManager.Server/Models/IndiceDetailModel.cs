using System;
using Trakx.Persistence.DAO;

namespace Trakx.IndiceManager.Server.Models
{
    public class IndiceDetailModel
    {
        public string Symbol { get; set; }

        public string Name { get; set; }

        public DateTime? CreationDate { get; set; }
    }
}
