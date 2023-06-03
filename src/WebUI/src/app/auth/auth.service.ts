import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { LoginRequest } from "./login-request";
import { Observable, Subject, tap } from "rxjs";
import { LoginResult } from "./login-result";

@Injectable({
    providedIn: 'root',
})

export class AuthService {
    constructor(
        protected http: HttpClient) {
    }

    public tokenKey: string = "token";
    private _authStatus = new Subject<boolean>();
    public authStatus = this._authStatus.asObservable();

    isAuthenticated() : boolean {
        return this.getToken() != null;
    }

    getToken() : string | null {
        return localStorage.getItem(this.tokenKey);
    }
    
    init() : void {
        if (this.isAuthenticated())
          this.setAuthStatus(true);
      } 

    login(item: LoginRequest): Observable<LoginResult> {
        const url = "api/Account/login";
        return this.http.post<LoginResult>(url, item)
            .pipe(tap(loginResult => {
                if(loginResult.success && loginResult.token) {
                    localStorage.setItem(this.tokenKey, loginResult.token);
                    this.setAuthStatus(true);
                }
            }));
    }

    logout() {
        localStorage.removeItem(this.tokenKey);
        this.setAuthStatus(false);
      }
    
      private setAuthStatus(isAuthenticated: boolean): void {
        this._authStatus.next(isAuthenticated);
      }
}