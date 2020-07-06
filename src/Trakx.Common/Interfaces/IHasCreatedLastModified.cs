using System;

namespace Trakx.Common.Interfaces
{
    public interface IHasCreatedLastModified
    {
        DateTime Created { get; set; }
        DateTime? LastModified { get; set; }
    }
}