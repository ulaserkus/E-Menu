using System;

namespace Shared.Kernel.Dtos
{
    public class MenuDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string Description { get; set; } // Optional field for additional information
    }
}
