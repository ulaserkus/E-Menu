import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { ChatBotComponent } from './chatbot/chatbot.component';

@Component({
  selector: 'app-layouts',
  templateUrl: './layouts.component.html',
  styleUrl: './layouts.component.css',
  standalone: true,
  imports: [RouterModule, NavbarComponent, FooterComponent, ChatBotComponent],
})
export class LayoutsComponent {}
