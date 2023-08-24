import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomValidators } from 'src/app/common/custom-validators';

@Component({
  selector: 'tt-user-change-password',
  templateUrl: './user-change-password.component.html',
  styleUrls: ['./user-change-password.component.scss']
})
export class UserChangePasswordComponent implements OnInit {
  userName!: string;
  form!: FormGroup;
  
  constructor(private userService: UserService,
    private router: Router,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.userName = this.activatedRoute.snapshot.paramMap.get('userName')!;
    this.initiateForm();
  }

  private initiateForm(): void {
    this.form = new FormGroup({
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

  onSubmit(): void {
    if(this.form.valid) {
      const newPassword = this.form.controls['password'].value;
      this.userService.changePassword(this.userName, newPassword)
        .subscribe(() => { this.router.navigate(['/users', this.userName]) } );
    } else {
      this.form.markAllAsTouched();
    }
  }
}
