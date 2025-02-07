import { ProductDetailsComponent } from './features/shop/product-details/product-details.component';
import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { ShopComponent } from './features/shop/shop.component';
import { TestErrorComponent } from './features/test-error/test-error.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { ServerErrorComponent } from './shared/components/server-error/server-error.component';
import { CartComponent } from './featuress/cart/cart.component';
import { CheckoutComponent } from './featuress/checkout/checkout.component';
import { LoginComponent } from './featuress/account/login/login.component';
import { RegisterComponent } from './featuress/account/register/register.component';
import { authGuard } from './core/guards/auth.guard';
import { emptyCartGuard } from './core/guards/empty-cart.guard';

export const routes: Routes = [
  {path:'', component: HomeComponent},
  {path:'shop', component: ShopComponent},
  {path:'shop/:id', component: ProductDetailsComponent},
  {path:'cart', component: CartComponent},
  {path:'checkout', component: CheckoutComponent, canActivate: [authGuard, emptyCartGuard]},
  {path:'account/login', component: LoginComponent},
  {path:'account/register', component: RegisterComponent},
  {path:'test-error', component: TestErrorComponent},
  {path:'notfound-error', component: NotFoundComponent},
  {path:'server-error', component: ServerErrorComponent},
  {path:'**', redirectTo: 'not-found', pathMatch: 'full'}
];
