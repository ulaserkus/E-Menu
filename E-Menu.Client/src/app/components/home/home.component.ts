import { Component } from '@angular/core';
import { MenuModel } from '../../models/menu.model';
import { HttpService } from '../../services/http.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  standalone: true,
  imports: [CommonModule, RouterLink]
})
export class HomeComponent {

  public menus: MenuModel[] = [];
  constructor(private readonly http: HttpService) { }

  ngOnInit() {
    this.getAll();
  }

  getAll() {
    this.http.post<MenuModel[]>('menus/all', {}, (res) => {
      this.menus = res.value || [];

    }, (err) => {
      console.error('Error fetching menus:', err);
    });
  }
}
