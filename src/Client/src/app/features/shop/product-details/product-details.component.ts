import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../../shared/models/product';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDivider } from '@angular/material/divider';

@Component({
  selector: 'app-product-details',
  imports: [
   CurrencyPipe,
   MatButton,
   MatInputModule,
   MatFormField,
   MatIcon,
   MatLabel,
   MatDivider,
   CommonModule
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit {

 private readonly shopService = inject(ShopService);
 private readonly activatedRoute = inject(ActivatedRoute);
 product?: Product;

 ngOnInit(): void {
  this.loadProdect();
}

loadProdect(){
  const id = this.activatedRoute.snapshot.paramMap.get('id');
  if(!id) return;
  this.shopService.getProduct(+id).subscribe({
   next: product => this.product = product,
   error :error => console.log(error)
  })
}
}
