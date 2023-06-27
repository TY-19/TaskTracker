import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { ActivatedRoute } from '@angular/router';
import { UserProfile } from 'src/app/models/user-profile';

@Component({
  selector: 'tt-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.scss']
})
export class UserDetailsComponent implements OnInit {

  userName!: string;
  user!: UserProfile;
  
  constructor(private userService: UserService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.userName = this.activatedRoute.snapshot.paramMap.get("userName")!;
    this.userService.getUser(this.userName)
      .subscribe(result => { 
        this.user = result;
      });
  }

}
