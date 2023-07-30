import { Component, OnInit } from '@angular/core';
import { LoginRequest } from '../models/login-request';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginResult } from '../models/login-result';

@Component({
  selector: 'tt-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss']
})
export class AuthComponent implements OnInit {
  
  title?: string;
  loginResult?: LoginResult;

  form!: FormGroup;

  constructor(
    private router: Router,
    private authService: AuthService
  ) { 

  }

  ngOnInit(): void {
    this.form = new FormGroup({
      nameOrEmail: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
    });    
  }

  onSubmit(): void {
    let loginRequest = <LoginRequest>{};
    loginRequest.nameOrEmail = this.form?.controls['nameOrEmail'].value;
    loginRequest.password = this.form?.controls['password'].value;

    this.authService
      .login(loginRequest)
      .subscribe({
        next: result => {
          this.loginResult = result;
          if (result.success && result.token) {
            this.router.navigate(["/"]).catch(error => console.log(error));
          }},
        error: error => {
          console.log(error);
          if (error.status == 401) {
            this.loginResult = error.error;
          }
        }
    });
  }

}
