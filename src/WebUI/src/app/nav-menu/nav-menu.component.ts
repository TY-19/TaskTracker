import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, takeUntil, } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'tt-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {
  
  private destroySubject = new Subject();
  isLoggedIn: boolean = false;
  username: string | null = null;

  constructor(public authService: AuthService,
    private router: Router) { 
      this.authService.authStatus
      .pipe(takeUntil(this.destroySubject))
      .subscribe({
        next:result => {
          this.isLoggedIn = result;
        }
      })
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(["/login"])
      .catch(error => {console.log(error)});
  }

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated();
    this.username = this.authService.getUserName();
    this.authService.userName
      .subscribe(username => this.username = username);
  }

  ngOnDestroy() {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }

}
