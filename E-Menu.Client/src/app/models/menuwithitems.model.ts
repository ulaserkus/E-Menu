import { MenuItemModel } from "./menu-item.model";

export class MenuWithItemsModel {
  id: string = "";
  name: string = "";
  code: string = "";
  currencyId: string = "";
  currencyName: string = "";
  description: string = "";
  items: MenuItemModel[] = [];
}