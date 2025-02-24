import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { MatStepperModule } from '@angular/material/stepper';
import { OrderSummaryComponent } from "../../shared/components/order-summary/order-summary.component";
import { MatButton } from '@angular/material/button';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { RouterLink } from '@angular/router';
import { StripeService } from '../../core/services/stripe.service';
import { StripeAddressElement } from '@stripe/stripe-js';
import { SnackbarService } from '../../core/services/snackbar.service';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { AccountService } from '../../core/services/account.service';
import { firstValueFrom } from 'rxjs';
import { Address } from '../../shared/models/user';
import { CheckoutDeliveryComponent } from "./checkout-delivery/checkout-delivery.component";

@Component({
  selector: 'app-checkout',
  imports: [
    OrderSummaryComponent,
    MatStepperModule,
    MatButton,
    RouterLink,
    MatCheckboxModule,
    CheckoutDeliveryComponent
],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit, OnDestroy {

   private readonly stripeService = inject(StripeService);
   adressElement?: StripeAddressElement;
   private readonly snackBar = inject(SnackbarService);
   private readonly accountService = inject(AccountService);
   saveAddress = false;

  async ngOnInit() {
   try {
      this.adressElement = await this.stripeService.createAdressElement();
      this.adressElement.mount('#address-element');
   } catch (error: any) {
      this.snackBar.error(error.message)
   }
  }

  ngOnDestroy(): void {
      this.stripeService.disposeElements();
  }

  async onStepChange(event: StepperSelectionEvent) {
    if(event.selectedIndex === 1) {
      if(this.saveAddress) {
        const address = await this.getAddressFromStripeAddress();
        address && firstValueFrom(this.accountService.updateAddress(address));
      }
    }

    if(event.selectedIndex === 2) {
       await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
    }
  }

  private async getAddressFromStripeAddress() : Promise<Address | null> {
    const result = await this.adressElement?.getValue();
    const stripeAddress = result?.value.address;

    if(stripeAddress) {
      return {
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
