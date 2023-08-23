import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, takeUntil, } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'tt-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit, OnDestroy {
  
  private destroySubject = new Subject();
  isLoggedIn: boolean = false;
  username: string | null = null;

  constructor(private authService: AuthService,
    private router: Router) { 
      this.getLoginStatus();
  }

  private getLoginStatus(): void {
    this.authService.authStatus
      .pipe(takeUntil(this.destroySubject))
      .subscribe(result => this.isLoggedIn = result);
  }

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated();
    this.username = this.authService.getUserName();
    this.authService.userName
      .subscribe(username => this.username = username);
  }

  ngOnDestroy(): void {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(["/login"])
      .catch(error => {console.log(error)});
  }

  get isAdmin(): boolean {
    return this.authService.isAdmin();
  }
}
