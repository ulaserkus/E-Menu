using Shared.Kernel.Interfaces;
using System;

namespace Shared.Kernel.Requests
{
    public class GetMenuProductCategoriesRequest : ICustomApiRequest
    {
        public Guid MenuId { get; set; }
    }
}
