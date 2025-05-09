import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatBadge } from '@angular/material/badge';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatMenuTrigger, MatMenu, MatMenuItem } from '@angular/material/menu';
import { MatDivider } from '@angular/material/divider';
import { Component, inject } from '@angular/core';
import { BusyService } from '../../../core/services/busy.service';
import { AccountService } from '../../../core/services/account.service';
import { CartService } from '../../../core/services/cart.service';


@Component({
  selector: 'app-header',
  imports: [
    MatIcon,
    MatButton,
    MatBadge,
    RouterLink,
    RouterLinkActive,
    MatProgressBar,
    MatMenuTrigger,
    MatMenu,
    MatDivider,
    MatMenuItem
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
