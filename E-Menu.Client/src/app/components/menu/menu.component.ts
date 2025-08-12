import { Component } from '@angular/core';
import { HttpService } from '../../services/http.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MenuWithItemsModel } from '../../models/menuwithitems.model';
import { ProductCategoryModel } from '../../models/product-category.model';
import { NavigationService } from '../../services/navigation.service';
import { MenuModel } from '../../models/menu.model';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { MenuItemPipe } from '../../pipes/menu-item.pipe';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, MenuItemPipe],
})
export class MenuComponent {
  constructor(
    private readonly http: HttpService,
    private readonly route: ActivatedRoute,
    private readonly navigation: NavigationService,
    private readonly sanitizer: DomSanitizer
  ) {}

  private routeSub!:Subscription;

  public search: string = '';
  public menuId: string = '';
  public menu: MenuWithItemsModel = new MenuWithItemsModel();
  public menus: MenuModel[] = [];
  public categories: ProductCategoryModel[] = [];
  public selectedCategory: ProductCategoryModel = new ProductCategoryModel();
  public filteredItems: MenuWithItemsModel = new MenuWithItemsModel();

  ngOnInit() {
    this.routeSub = this.route.params.subscribe(params => {
      this.menuId = params['id'];
      if (this.menuId) {
        this.getAllMenus();
        this.getCategories();
        this.getMenu();
      } else {
        this.navigation.navigateToHome();
      }
    });
  }

  getSafeImage(base64Data: string): SafeUrl {
    return this.sanitizer.bypassSecurityTrustUrl(
      `data:image/jpeg;base64,${base64Data}`
    );
  }

  getMenu() {
    this.http.post<MenuWithItemsModel>(
      'menus/items',
      {
        menuId: this.menuId,
      },
      (res) => {
        this.menu = res.value || new MenuWithItemsModel();
        this.filterItems(); // Menü yüklendiğinde filtrele
      },
      (err) => {
        console.error('Error fetching menus:', err);
        alert('Menü yükleme hatası oluştu.');
      }
    );
  }

  getAllMenus() {
    this.http.post<MenuModel[]>(
      'menus/all',
      {},
      (res) => {
        this.menus = res.value || [];
      },
      (err) => {
        console.error('Error fetching menus:', err);
        alert('Menü listesi yüklenirken hata oluştu.');
      }
    );
  }

  getCategories() {
    this.http.post<ProductCategoryModel[]>(
      'menus/categories',
      {
        menuId: this.menuId,
      },
      (res) => {
        this.categories = res.value || [];
        if (this.categories.length > 0) {
          this.selectedCategory = this.categories[0];
          this.filterItems(); // Kategoriler yüklendiğinde filtrele
        }
      },
      (err) => {
        console.error('Error fetching categories:', err);
        alert('Kategori yükleme hatası oluştu.');
      }
    );
  }

  filterItems(): void {
    if (!this.menu || !this.menu.items) {
      this.filteredItems.items = [];
      return;
    }

    const searchLower = this.search.toLowerCase();

    this.filteredItems.items = this.menu.items.filter((item) => {
      const matchesSearch =
        item.product.name.toLowerCase().includes(searchLower) ||
        item.description.toLowerCase().includes(searchLower);

      const matchesCategory =
        this.selectedCategory.id === 'all' ||
        item.product.productCategory.id === this.selectedCategory.id;

      return matchesSearch && matchesCategory;
    });
  }

  // Kategori değiştiğinde tetiklenecek yöntem
  onCategoryChange(category: ProductCategoryModel): void {
    this.selectedCategory = category;
    this.filterItems();
  }

  // TrackBy fonksiyonu performans için
  trackById(index: number, item: any): string {
    return item.id;
  }
}
