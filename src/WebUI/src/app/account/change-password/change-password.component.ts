import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../account.service';
import { CustomValidators } from 'src/app/common/custom-validators';

@Component({
  selector: 'tt-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {

  form!: FormGroup;

  isSuccessful: boolean | null = null;
  errors?: string;
  
  constructor(private router: Router,
    private accountService: AccountService) { 
      
    }

  ngOnInit(): void {
    this.initiateForm();
  }

  initiateForm() {
    this.form = new FormGroup({
      oldPassword: new FormControl("", [
        Validators.required
      ]),
      password: new FormControl("", [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(20)
      ]),
      passwordConfirm: new FormControl("", [
        Validators.required
      ])
    },
    {
      validators: CustomValidators.passwordMatchValidator()
    });
  }

  onSubmit() {
    if (this.form.valid)
    {
      let passwordModel = {
        oldPassword: this.form.controls['oldPassword'].value,
        newPassword: this.form.controls['password'].value
      }
    
      this.accountService.changePassword(passwordModel)
        .subscribe({
          next: response => {
            if (response.status == 204) {
              this.isSuccessful = true;
              this.router.navigate(['/profile'])
                .catch(error => console.log(error));
            }
          },
          error: response => {
            this.isSuccessful = false;
            if (typeof(response.error) === 'string') {
              this.errors = response.error;
            } else {
              this.errors = "Make sure that you use the correct old password!";
            }
          }});
    } else {
      this.form.markAllAsTouched();
    }
  }
}
