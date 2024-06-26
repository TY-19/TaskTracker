import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth.service';
import { RegistrationResult } from '../../models/registration-result';
import { RegistrationRequest } from '../../models/registration-request';
import { Router } from '@angular/router';
import { CustomValidators } from 'src/app/common/custom-validators';

@Component({
  selector: 'tt-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit {

  form!: UntypedFormGroup;
  registrationResult?: RegistrationResult;

  constructor(private router: Router,
    private authService: AuthService) { 

  }

  ngOnInit(): void {
    this.initiateForm();
  }

  private initiateForm(): void {
    this.form = new UntypedFormGroup({
      userName: new UntypedFormControl('', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(25),
        Validators.pattern("^[a-zA-Z0-9_]*$")
      ]),
      email: new UntypedFormControl('', [
        Validators.required, 
        Validators.email
      ]),
      password: new UntypedFormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(20)
      ]),
      passwordConfirm: new UntypedFormControl('', Validators.required),
    },
    { 
      validators: CustomValidators.passwordMatchValidator()
    }
    );
  }

  onSubmit(): void {
    if(this.form.valid) {
      const registrationRequest: RegistrationRequest = {
        userName: this.form.controls['userName'].value,
        email: this.form.controls['email'].value,
        password: this.form.controls['password'].value
      }

      this.authService.registration(registrationRequest)
        .subscribe(result => {
          if(result.success) {
            this.router.navigate(["/"]);
          } else {
            this.registrationResult = result;
          }
      });
    } else {
      this.form.markAllAsTouched();
    }
  }
}
