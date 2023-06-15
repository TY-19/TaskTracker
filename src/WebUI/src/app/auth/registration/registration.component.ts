import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth.service';
import { RegistrationResult } from './registration-result';
import { RegistrationRequest } from './registration-request';
import { Router } from '@angular/router';

@Component({
  selector: 'tt-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit {

  form!: FormGroup;

  registrationResult?: RegistrationResult;

  constructor(private router: Router,
    private authService: AuthService) { 

  }

  ngOnInit(): void {
    this.initiateForm();
  }

  private initiateForm() {
    this.form = new FormGroup({
      userName: new FormControl('', Validators.required),
      email: new FormControl('', [
        Validators.required, 
        Validators.email
      ]),
      password: new FormControl('', Validators.required),
      passwordConfirm: new FormControl('', Validators.required),
    },
    { 
      validators: this.authService.passwordMatchValidator()
    }
    );
  }

  onSubmit(): void {
    let registrationRequest: RegistrationRequest = {
      userName: this.form.controls['userName'].value,
      email: this.form.controls['email'].value,
      password: this.form.controls['password'].value
    }

    this.authService.registration(registrationRequest)
      .subscribe((result) => {
        if(result.success)
        {
          this.router.navigate(["/"])
            .catch(error => console.log(error));
        }
        else
        {
          this.registrationResult = result;
        }
      });
  }

}
