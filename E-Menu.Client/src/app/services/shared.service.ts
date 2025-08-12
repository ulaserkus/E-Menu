import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  private searchSubject = new BehaviorSubject<string>(''); // Başlangıç değeri boş bir string
  search$ = this.searchSubject.asObservable(); // Observable olarak expose et

  setSearch(value: string): void {
    this.searchSubject.next(value); // Yeni bir değer ayarla
  }
}