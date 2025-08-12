import { Pipe, PipeTransform } from '@angular/core';
import { MenuItemModel } from '../models/menu-item.model';
import { ProductCategoryModel } from '../models/product-category.model';

@Pipe({
  name: 'menuItem',
  standalone: true,
})
export class MenuItemPipe implements PipeTransform {
  transform(
    value: MenuItemModel[],
    search: string,
    category: ProductCategoryModel
  ): MenuItemModel[] {
    if (!value || !search) {
      return value; // Eğer liste veya arama boşsa, olduğu gibi döndür
    }

    const searchLower = search.toLowerCase();

    return value.filter((item) => {
      const matchesSearch =
        (item.product.name?.toLowerCase().includes(searchLower) ?? false) ||
        (item.description?.toLowerCase().includes(searchLower) ?? false);

      const matchesCategory =
        !category ||
        !category.id ||
        item.product.productCategory.id === category.id;

      return matchesSearch && matchesCategory;
    });
  }
}
