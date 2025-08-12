import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class NavigationService {

  constructor( private readonly router: Router ) { }

  navigateToMenu(menuId: string): void {
  
    if (menuId) {
      this.router.navigate(['/menu', menuId]);
    } else {
      this.navigateToHome();
    }
  }

  navigateToHome(): void {
    this.router.navigate(['/']);
  }
}
