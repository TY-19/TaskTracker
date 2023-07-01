import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserProfile } from 'src/app/models/user-profile';
import { RolesService } from '../roles.service';

@Component({
  selector: 'tt-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.scss']
})
export class UserDetailsComponent implements OnInit {

  userName!: string;
  user!: UserProfile;
  
  constructor(private userService: UserService,
    public rolesService: RolesService,
    private activatedRoute: ActivatedRoute,
    private router: Router) { 

  }

  ngOnInit(): void {
    this.userName = this.activatedRoute.snapshot.paramMap.get("userName")!;
    this.userService.getUser(this.userName)
      .subscribe(result => { 
        this.user = result;
      });
  }

  onDeleteUser() {
    this.userService.deleteUser(this.userName)
      .subscribe(() => {
        this.router.navigate(['/users']);
      });
  }
}
