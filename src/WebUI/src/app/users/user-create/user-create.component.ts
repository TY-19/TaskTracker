import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RegistrationResult } from 'src/app/models/registration-result';
import { CustomValidators } from 'src/app/common/custom-validators';
import { RegistrationRequest } from 'src/app/models/registration-request';
import { Router } from '@angular/router';

@Component({
  selector: 'tt-user-create',
  templateUrl: './user-create.component.html',
  styleUrls: ['./user-create.component.scss']
})
export class UserCreateComponent implements OnInit {

  form!: FormGroup;
  registrationResult?: RegistrationResult;

  constructor(private userService: UserService,
    private router: Router) { 

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
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(8)
      ]),
      passwordConfirm: new FormControl('', Validators.required),
    },
    { 
      validators: CustomValidators.passwordMatchValidator()
    }
    );
  }

  onSubmit() {
    let registrationRequest: RegistrationRequest = {
      userName: this.form.controls['userName'].value,
      email: this.form.controls['email'].value,
      password: this.form.controls['password'].value
    }
    
    this.userService.addUser(registrationRequest)
      .subscribe(result => {
        if(result.success) {
          this.router.navigate(["/users", registrationRequest.userName])
            .catch(error => console.log(error));
        } else {
          this.registrationResult = result;
        }
      });
  }

}
