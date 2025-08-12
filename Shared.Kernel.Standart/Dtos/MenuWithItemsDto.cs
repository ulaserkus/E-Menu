using System;
using System.Collections.Generic;

namespace Shared.Kernel.Dtos
{
    public class MenuWithItemsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid? CurrencyId { get; set; }
        public string CurrencyName { get; set; }

        public string Description { get; set; } // Optional field for additional information
        public IEnumerable<MenuItemDto> Items { get; set; }
    }
}
