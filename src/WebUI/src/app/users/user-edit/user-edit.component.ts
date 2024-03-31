import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { UserService } from '../user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RolesService } from '../roles.service';
import { UserUpdateModel } from 'src/app/models/update-models/user-update-model';

@Component({
  selector: 'tt-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user-edit.component.scss']
})
export class UserEditComponent implements OnInit {

  userName!: string;
  allRoles!: string[];
  form!: UntypedFormGroup;

  constructor(private userService: UserService,
    private rolesService: RolesService,
    private router: Router,
    private activatedRoute: ActivatedRoute) { 
    
  }

  ngOnInit(): void {
    this.userName = this.activatedRoute.snapshot.paramMap.get('userName')!;
    this.loadRoles();
    this.initiateForm();
    this.loadUser();
  }

  private loadRoles(): void {
    this.rolesService.getAllRoles()
      .subscribe(result => this.allRoles = result);
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
      roles: new UntypedFormControl(''),
      firstName: new UntypedFormControl('', [
        Validators.required,
        Validators.maxLength(50)
      ]),
      lastName: new UntypedFormControl('', [
        Validators.required,
        Validators.maxLength(50)
      ])
    });
  }

  private loadUser(): void {
    this.userService.getUser(this.userName)
      .subscribe(result => this.form.patchValue(result));
  }

  onSubmit(): void {
    if(this.form.valid) {
      const updateUserModel: UserUpdateModel = {
        userName: this.form.controls['userName'].value,
        email: this.form.controls['email'].value,
        roles: this.form.controls['roles'].value,
        firstName: this.form.controls['firstName'].value,
        lastName: this.form.controls['lastName'].value
      };
      const userName = updateUserModel.userName ?? this.userName;
      this.userService.updateUser(this.userName, updateUserModel)
        .subscribe(() => {
          this.router.navigate(['/users', userName]);
        });
    } else {
      this.form.markAllAsTouched();
    }
  }
}
