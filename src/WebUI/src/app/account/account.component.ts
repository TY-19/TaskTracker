import { Component, OnInit } from '@angular/core';
import { AccountService } from './account.service';
import { UserProfile } from '../models/user-profile';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'tt-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {

  profile! : UserProfile;

  form!: FormGroup;

  updateResult: boolean | null = null;
  updateErrors?: string;

  constructor(private router: Router,
    private accountService: AccountService) { 

  }

  ngOnInit(): void {
    this.getUserProfile();
    this.initiateForm();
    this.loadProfile();
  }

  private initiateForm()
  {
    this.form = new FormGroup({
      id: new FormControl("", [
        Validators.required
      ]),
      userName: new FormControl("", [
        Validators.required
      ]),
      email: new FormControl("", [
        Validators.required
      ]),
      firstName: new FormControl(),
      lastName: new FormControl()
    });
  }

  private getUserProfile() {
    this.accountService.getUserProfile()
      .subscribe(result => this.profile = result);
  }

  private loadProfile() {
    this.accountService.getUserProfile()
      .subscribe(result => {
        this.profile = result;
        this.form.patchValue(result);
      });
  }

  onSubmit() {
    if(this.form.valid)
    {
      let profile = {
        username: this.form.controls['userName'].value,
        email: this.form.controls['email'].value,
        firstName: this.form.controls['firstName'].value,
        lastName: this.form.controls['lastName'].value
      }
      this.updateProfile(profile)
    }
  }

  updateProfile(profile: any) {
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
