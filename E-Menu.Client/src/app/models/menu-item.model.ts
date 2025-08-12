import { MenuProductModel } from "./menu-product.model";

export class MenuItemModel {
  id: string = '';
  itemCode: string = '';
  price: number = 0;
  formattedPrice: string = '';
  currencyName: string = '';
  currencyId: string = '';
  description: string = '';
  product : MenuProductModel = new MenuProductModel();
}
