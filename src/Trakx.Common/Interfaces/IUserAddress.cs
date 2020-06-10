using System;
using System.Collections.Generic;
using System.Text;

namespace Trakx.Common.Interfaces
{
    public interface IUserAddress
    {
        public string Id { get; }

        public string ChainId { get; }
        public string UserId { get; }
        public string Address { get; }
    }
}
