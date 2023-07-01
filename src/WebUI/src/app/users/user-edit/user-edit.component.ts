import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
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
  form!: FormGroup;
  allRoles!: string[];

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

  private loadRoles() {
    this.rolesService.getAllRoles()
      .subscribe(result => this.allRoles = result);
  }

  private initiateForm() {
    this.form = new FormGroup({
      userName: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(25),
        Validators.pattern("^[a-zA-Z0-9_]*$")
      ]),
      email: new FormControl('', [
        Validators.required, 
        Validators.email
      ]),
      roles: new FormControl(''),
      firstName: new FormControl(''),
      lastName: new FormControl('')
    });
  }

  loadUser() {
    this.userService.getUser(this.userName)
      .subscribe(result => {
        this.form.patchValue(result);
      });
  }

  onSubmit() {
    if(this.form.valid) {
      let updateUserModel: UserUpdateModel = {
        userName: this.form.controls['userName'].value,
        email: this.form.controls['email'].value,
        roles: this.form.controls['roles'].value,
        firstName: this.form.controls['firstName'].value,
        lastName: this.form.controls['lastName'].value
      };
      let userName = updateUserModel.userName ?? this.userName;
      this.userService.updateUser(this.userName, updateUserModel)
        .subscribe(() => {
          this.router.navigate(['/users', userName]);
        });
    }
  }

}
