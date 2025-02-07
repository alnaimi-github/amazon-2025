import { BusyService } from './../../../core/services/busy.service';
import { Component, inject } from '@angular/core';
import {  MatBadge } from '@angular/material/badge';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatProgressBar } from '@angular/material/progress-bar';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CartService } from '../../../core/services/cart.service';
import { AccountService } from '../../../core/services/account.service';


@Component({
  selector: 'app-header',
  imports: [
    MatIcon,
    MatButton,
    MatBadge,
    RouterLink,
    RouterLinkActive,
    MatProgressBar

  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {

  busyService = inject(BusyService);
  cartService = inject(CartService);
  accountService = inject(AccountService);
  private readonly router = inject(Router);

logout() {
  this.accountService.logout().subscribe({
  next: () => {
    this.accountService.currentUser.set(null);
    this.router.navigateByUrl('/');
  }
  });
}

}
