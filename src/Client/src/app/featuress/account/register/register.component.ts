import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AccountService } from '../../../core/services/account.service';
import { Router } from '@angular/router';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';

@Component({
  selector: 'app-register',
  imports: [
      ReactiveFormsModule,
      MatCard,
      MatButton,
     TextInputComponent,
     CommonModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
 private readonly fb = inject(FormBuilder);
 private readonly accountService = inject(AccountService);
 private readonly route = inject(Router);
 private readonly snack = inject(SnackbarService);
 validationErrors?: string[];

 registerForm = this.fb.group({
  firstName: ['', Validators.required],
  lastName: ['', Validators.required],
  email: ['', Validators.required, Validators.email],
  password: ['', Validators.required]
 });

 onSubmit() {
  this.accountService.register(this.registerForm.value).subscribe({
    next: () => {
      this.snack.success("Registeration successful - you can now login");
      this.route.navigateByUrl('/account/login');
    },
    error: errors => this.validationErrors = errors,

  });
 }
}
