using Shared.Kernel.Interfaces;
using System;

namespace Shared.Kernel.Requests
{
    public class GetMenuWithItemsRequest : ICustomApiRequest
    {
        public Guid MenuId { get; set; }
    }
}
