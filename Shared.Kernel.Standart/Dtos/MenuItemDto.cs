using System;

namespace Shared.Kernel.Dtos
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; }

        public decimal Price { get; set; }
        public string FormattedPrice { get; set; }

        public Guid? CurrencyId { get; set; }
        public string CurrencyName { get; set; }

        public string Description { get; set; }
        public MenuProductDto Product { get; set; }
    }
}
