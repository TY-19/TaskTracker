import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router,
    private authService: AuthService) {

  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):
    Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      if (this.authService.isAuthenticated() && this.doesRoleHaveAccess(route)) {
        return true;
      }
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
      return false;
  }

  private doesRoleHaveAccess(route: ActivatedRouteSnapshot) : boolean {
    let roles = route.data['roles'] as Array<string>;
    if(!roles || roles.length === 0) {
      return true;
    }
    return roles.some(r => this.authService.getRoles().includes(r));
  }
}
