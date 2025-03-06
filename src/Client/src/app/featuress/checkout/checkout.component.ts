import { DeliveryMethod } from './../../shared/models/deliveryMethod';
import { Component, inject, OnDestroy, OnInit , signal } from '@angular/core';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { OrderSummaryComponent } from "../../shared/components/order-summary/order-summary.component";
import { MatButton } from '@angular/material/button';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule  } from '@angular/material/progress-spinner';
import { Router, RouterLink } from '@angular/router';
import { StripeService } from '../../core/services/stripe.service';
import { StripeAddressElement, ConfirmationToken , StripeAddressElementChangeEvent, StripePaymentElement, StripePaymentElementChangeEvent } from '@stripe/stripe-js';
import { SnackbarService } from '../../core/services/snackbar.service';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { AccountService } from '../../core/services/account.service';
import { firstValueFrom } from 'rxjs';
import { Address } from '../../shared/models/user';
import { CheckoutDeliveryComponent } from "./checkout-delivery/checkout-delivery.component";
import { CheckoutReviewComponent } from "./checkout-review/checkout-review.component";
import { CartService } from '../../core/services/cart.service';
import { CurrencyPipe } from '@angular/common';
import { OrderToCreate, ShippingAddress } from '../../shared/models/order';
import { OrderService } from '../../core/services/order.service';

@Component({
  selector: 'app-checkout',
  imports: [
    OrderSummaryComponent,
    MatStepperModule,
    MatButton,
    RouterLink,
    MatCheckboxModule,
    CheckoutDeliveryComponent,
    CheckoutReviewComponent,
    CurrencyPipe,
   MatProgressSpinnerModule
],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit, OnDestroy {

   private readonly stripeService = inject(StripeService);
   adressElement?: StripeAddressElement;
   paymentElement?: StripePaymentElement;
   private readonly snackBar = inject(SnackbarService);
   private readonly accountService = inject(AccountService);
   private router = inject(Router);
   cartService = inject(CartService);
   orderService = inject(OrderService);
   saveAddress = false;
   completionStatus = signal<{address: boolean, card: boolean, delivery: boolean}>(
    {address: false, card: false, delivery: false});

    confirmationToken?: ConfirmationToken;
    loading = false;

  async ngOnInit() {
   try {
      this.adressElement = await this.stripeService.createAdressElement();
      this.adressElement.mount('#address-element');
      this.adressElement.on('change', this.handleAddressChange);

      this.paymentElement = await this.stripeService.createPaymentElement();
      this.paymentElement.mount('#payment-element');
      this.paymentElement.on('change', this.handlePaymentChange);

   } catch (error: any) {
      this.snackBar.error(error.message)
   }
  }

  handleAddressChange = (event: StripeAddressElementChangeEvent) => {
    this.completionStatus.update(state => {
      state.address = event.complete;
      return state;
    })
  }

  handlePaymentChange = (event: StripePaymentElementChangeEvent) => {
    this.completionStatus.update(state => {
      state.card = event.complete;
      return state;
    })
  }

  handleDeliveryChange(event: boolean) {
    this.completionStatus.update(state => {
      state.delivery = event;
      return state;
    })
  }

  ngOnDestroy(): void {
      this.stripeService.disposeElements();
  }

  async confirmPayment(stepper: MatStepper) {
    this.loading = true;
  try {
    if(this.confirmationToken) {
      const result = await this.stripeService.confirmPayment(this.confirmationToken);

      if (result.paymentIntent?.status === 'succeeded') {
        const order = await this.CreateOrderModel();
        const orderResult = await firstValueFrom(this.orderService.createOrder(order));
        if (orderResult) {
          this.cartService.deleteCart();
          this.cartService.selectedDelivery.set(null);
          this.router.navigateByUrl('/checkout/success');
        } else {
          throw new Error('Order creation failed.');
        }
      } else if (result.error) {
        throw new Error(result.error.message);
      } else {
        throw new Error('Something went wrong.');
      }
    }
  } catch (error: any) {
    this.snackBar.error(error.message || 'Something went wrong');
    stepper.previous();
  } finally {
    this.loading = false;
  }
  }

 async getConfirmationToken() {
  try {
    if (Object.values(this. completionStatus()).every(status => status === true) ) {
    const result = await this.stripeService.createConfrimationToken();
    if (result.error) throw new Error(result.error.message);
    this.confirmationToken = result.confirmationToken;
    console. log(this. confirmationToken);
    }
  } catch (error: any) {
    this.snackBar.error(error.message);
    }
 }

  async onStepChange(event: StepperSelectionEvent) {
    if(event.selectedIndex === 1) {
      if(this.saveAddress) {
        const address = await this.getAddressFromStripeAddress() as Address;
        address && firstValueFrom(this.accountService.updateAddress(address));
      }
    }

    if(event.selectedIndex === 2) {
       await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
    }

    if(event.selectedIndex === 3 ) {
      await this.getConfirmationToken();
   }
  }

  private async CreateOrderModel() : Promise<OrderToCreate> {
    const cart = this.cartService.cart();
    const shippingAddress = await this.getAddressFromStripeAddress() as ShippingAddress;
    const card = this.confirmationToken?.payment_method_preview.card;
    if (!cart ?. id || !cart.deliveryMethodId || !card || !shippingAddress) {
      throw new Error('Problem creating order');
    }

    return {
      cartId: cart.id,
      paymentSummary: {
      last4: +card.last4,
      brand: card.brand,
      expMonth: card.exp_month,
      expYear: card.exp_year,
    },
      deliveryMethodId: cart.deliveryMethodId,
      shippingAddress
    }
  }

  private async getAddressFromStripeAddress() : Promise<Address | ShippingAddress | null> {
    const result = await this.adressElement?.getValue();
    const stripeAddress = result?.value.address;

    if(stripeAddress) {
      return {
        name: result.value.name,
        line1: stripeAddress.line1,
        line2: stripeAddress.line2,
        city: stripeAddress.city,
        country: stripeAddress.country,
        postalCode: stripeAddress.postal_code,
        state: stripeAddress.state
      } as Address;
    } else return null;
  }

  onSaveAddressCheckboxChange(event: MatCheckboxChange) {
     this.saveAddress = event.checked;
  }
}
