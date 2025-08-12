using System;

namespace Shared.Kernel.Dtos
{
    public class MenuProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public bool InStock { get; set; }
        public ProductCategoryDto ProductCategory { get; set; }
    }
}
