import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../account.service';
import { CustomValidators } from 'src/app/common/custom-validators';
import { ChangePasswordModel } from 'src/app/models/update-models/change-password.model';

@Component({
  selector: 'tt-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {

  form!: UntypedFormGroup;

  isSuccessful: boolean | null = null;
  errors?: string;
  
  constructor(private router: Router,
    private accountService: AccountService) { 
      
    }

  ngOnInit(): void {
    this.initiateForm();
  }

  private initiateForm(): void {
    this.form = new UntypedFormGroup({
      oldPassword: new UntypedFormControl("", [
        Validators.required
      ]),
      password: new UntypedFormControl("", [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(20)
      ]),
      passwordConfirm: new UntypedFormControl("", [
        Validators.required
      ])
    },
    {
      validators: CustomValidators.passwordMatchValidator()
    });
  }

  onSubmit(): void {
    if (this.form.valid)
    {
      const passwordModel: ChangePasswordModel = {
        oldPassword: this.form.controls['oldPassword'].value,
        newPassword: this.form.controls['password'].value
      }
      this.changePassword(passwordModel);
    } else {
      this.form.markAllAsTouched();
    }
  }

  private changePassword(passwordModel: ChangePasswordModel): void {
    this.accountService.changePassword(passwordModel)
      .subscribe({
        next: response => {
          if (response.status == 204) {
            this.isSuccessful = true;
            this.router.navigate(['/profile']);
          }
        },
        error: response => {
          this.isSuccessful = false;
          this.errors = response.error;
        }
      });
  }
}
