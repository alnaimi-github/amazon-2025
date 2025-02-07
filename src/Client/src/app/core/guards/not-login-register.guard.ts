import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';

export const notLoginRegisterGuard: CanActivateFn = (route, state) => {

  const accountService = inject(AccountService);
  const router = inject(Router);

  if(accountService.currentUser()) {
    router.navigateByUrl('/notfound-error');
    return false;
  }

  return true;
};
