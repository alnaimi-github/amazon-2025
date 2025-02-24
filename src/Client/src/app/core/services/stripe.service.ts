import { AccountService } from './account.service';
import { inject, Injectable } from '@angular/core';
import { loadStripe, Stripe, StripeAddressElement, StripeAddressElementOptions,StripeElements } from '@stripe/stripe-js';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CartService } from './cart.service';
import { Cart } from '../../shared/models/cart';
import { firstValueFrom, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StripeService {
  baseUrl = environment.apiUrl;
 private readonly http = inject(HttpClient);
 private readonly cartService = inject(CartService);
 private readonly stripePromise?: Promise<Stripe | null>;
 private  elements?: StripeElements;
 private addressElement?: StripeAddressElement;
 private readonly accountService = inject(AccountService);

  constructor() {
      this.stripePromise = loadStripe(environment.stripePublicKey);
  }

  getStripeInstance() {
    return this.stripePromise;
  }

  createOrUpdatePaymentIntent() {
    const cart = this.cartService.cart();
    if(!cart) throw new Error('Problem with cart');
    return this.http.post<Cart>(this.baseUrl + 'payments/' + cart.id, {}).pipe(
      map(cart => {
        this.cartService.SetCart(cart);
        return cart;
      })
    )
  }

  async createAdressElement() {
    if(!this.addressElement) {
      const elements = await this.initializeElements();
      if(elements) {
        const user = this.accountService.currentUser();
        let defaultValues: StripeAddressElementOptions['defaultValues'] = {};

        if(user) {
          defaultValues.name= user.firstName + ' ' + user.lastName;
        }

        if(user?.address) {
          defaultValues.address = {
            line1: user.address.line1,
            line2: user.address.line2,
            country: user.address.country,
            city: user.address.city,
            state: user.address.state,
            postal_code: user.address.postalCode
          };
        }

        const options: StripeAddressElementOptions = {
          mode: 'shipping',
          defaultValues
        }
        this.addressElement = elements.create('address', options);
      } else {
        throw new Error('Elements instance has not been loaded');
      }
    }
    return this.addressElement;
  }

  async initializeElements() {
    if(!this.elements) {
       const stripe = await this.getStripeInstance();
       if(stripe) {
        const cart = await firstValueFrom(this.createOrUpdatePaymentIntent());
        this.elements = stripe.elements(
          {clientSecret: cart.clientSecret,appearance: {labels: 'floating'}})
       } else {
        throw new Error('Stripe has not been loaded')
       }
    }
    return this.elements;
  }

  disposeElements() {
    this.elements = undefined;
    this.addressElement = undefined;
  }
}
