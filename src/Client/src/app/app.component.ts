import { HttpClient } from '@angular/common/http';
import { Component, inject, Inject, OnInit } from '@angular/core';
import { HeaderComponent } from "./layout/header/header/header.component";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [
    HeaderComponent,
    CommonModule
],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'] 
})
export class AppComponent implements OnInit {
  baseUrl = 'https://localhost:5000/api/';
  private http = inject(HttpClient);
  products: any[] = [];
  title = 'Amazon';
  ngOnInit(): void {
    this.http.get<any>(this.baseUrl + 'products').subscribe( {
      next: response => this.products = response.data,
      error: error => console.error(error)
    });
  }

}