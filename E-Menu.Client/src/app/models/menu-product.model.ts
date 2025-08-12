import { ProductCategoryModel } from "./product-category.model";

export class MenuProductModel {
  id: string = ''
  name: string = '';
  imageUrl: string = '';
  inStock: boolean = false;
  productCategory: ProductCategoryModel = new ProductCategoryModel();
}