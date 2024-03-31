import { Component, OnInit } from '@angular/core';
import { AccountService } from './account.service';
import { UserProfile } from '../models/user-profile';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { UserUpdateModel } from '../models/update-models/user-update-model';

@Component({
  selector: 'tt-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {

  form!: UntypedFormGroup;
  profile! : UserProfile;
  updateResult: boolean | null = null;
  updateErrors?: string;

  constructor(private accountService: AccountService) {

  }

  ngOnInit(): void {
    this.initiateForm();
    this.loadProfile();
  }

  private initiateForm(): void
  {
    this.form = new UntypedFormGroup({
      id: new UntypedFormControl("", [
        Validators.required
      ]),
      userName: new UntypedFormControl("", [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(25),
        Validators.pattern("^[a-zA-Z0-9_]*$")
      ]),
      email: new UntypedFormControl("", [
        Validators.required,
        Validators.email
      ]),
      firstName: new UntypedFormControl("", [
        Validators.required,
        Validators.maxLength(50)
      ]),
      lastName: new UntypedFormControl("", [
        Validators.required,
        Validators.maxLength(50)
      ])
    });
  }

  private loadProfile(): void {
    this.accountService.getUserProfile()
      .subscribe(result => {
        this.profile = result;
        this.form.patchValue(result);
      });
  }

  onSubmit(): void {
    if(this.form.valid)
    {
      const profile: UserUpdateModel = this.getProfileFromForm();
      this.updateProfile(profile)
    } else {
      this.form.markAllAsTouched();
    }
  }

  private getProfileFromForm(): UserUpdateModel {
    return {
      username: this.form.controls['userName'].value,
      email: this.form.controls['email'].value,
      firstName: this.form.controls['firstName'].value,
      lastName: this.form.controls['lastName'].value
    }
  }

  private updateProfile(profile: UserUpdateModel) {
    this.accountService.updateUserProfile(profile)
      .subscribe({
        next: response => {
        if (response.status == 204)
          this.updateResult = true;
      },
        error: response => {
          this.updateResult = false;
          this.updateErrors = response.error;
        }
    });
  }
}
