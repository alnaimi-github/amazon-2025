import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { DeliveryMethod } from '../../shared/models/deliveryMethod';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
 baseUrl = environment.apiUrl;
 private readonly http = inject(HttpClient);
 deliveryMethods: DeliveryMethod[] = [];

  getDeliveryMethods() {
   if(this.deliveryMethods.length > 0) {
     return of(this.deliveryMethods);
   }
    return this.http.get<DeliveryMethod[]>(this.baseUrl + 'payments/delivery-Methods').pipe(
      map(methods => {
        this.deliveryMethods = methods.sort((a, b) => b.price - a.price);
        return methods;
      })
    );
  }
}
