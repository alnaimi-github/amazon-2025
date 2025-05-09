import { Component, inject, input } from '@angular/core';
import { CartItem } from '../../../shared/models/cart';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import {MatIconModule } from '@angular/material/icon';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-cart-item',
  imports: [
    RouterLink,
    CurrencyPipe,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.scss'
})
export class CartItemComponent {
 item = input.required<CartItem>();
 cartService = inject(CartService);

 incrementQuantity() {
  this.cartService.addItemToCart(this.item());
 }

 decrementQuantity() {
  this.cartService.removeItemFromCart(this.item().productId);
 }

 removeItemFromCart() {
  this.cartService.removeItemFromCart(this.item().productId, this.item().quantity);
 }
}
