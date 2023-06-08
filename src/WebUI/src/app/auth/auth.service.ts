import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { LoginRequest } from "./login-request";
import { Observable, Subject, tap } from "rxjs";
import { LoginResult } from "./login-result";
import { DefaultRolesNames } from "../config/default-roles-names";

@Injectable({
    providedIn: 'root',
})

export class AuthService {
    
    public tokenKey: string = "token";
    private _authStatus = new Subject<boolean>();
    public authStatus = this._authStatus.asObservable();
    
    private rolesKey: string = "roles";
    private userRoles: string[] = [];

    constructor(
        protected http: HttpClient) {
            this.init();
    }

    init() : void {
        this.setAuthStatus(this.isAuthenticated());
        this.userRoles = this.getRoles();
    }

    setAuthStatus(isAuthenticated: boolean): void {
        this._authStatus.next(isAuthenticated);
    }

    isAuthenticated() : boolean {
        return this.getToken() != null;
    }

    getToken() : string | null {
        return localStorage.getItem(this.tokenKey);
    }

    getRoles() : string[] {
        let roles = localStorage.getItem(this.rolesKey);
        if (roles) return JSON.parse(roles);
        else return [];
    }    

    login(item: LoginRequest): Observable<LoginResult> {
        const url = "api/Account/login";
        return this.http.post<LoginResult>(url, item)
            .pipe(tap(loginResult => {
                if(loginResult.success && loginResult.token) {
                    localStorage.setItem(this.tokenKey, loginResult.token);
                    this.setAuthStatus(true);
                    localStorage.setItem(this.rolesKey, JSON.stringify(loginResult.roles));
                    this.userRoles = loginResult.roles;
                }
            }));
    }

    logout() {
        localStorage.removeItem(this.tokenKey);
        localStorage.removeItem(this.rolesKey);
        this.setAuthStatus(false);
        this.userRoles = [];
      }

    isAdmin() : boolean {
        return this.userRoles.includes(DefaultRolesNames.DEFAULT_ADMIN_ROLE);
    }

    isManager() : boolean {
        return this.userRoles.includes(DefaultRolesNames.DEFAULT_MANAGER_ROLE);
    }

    isEmployee() : boolean {
        return this.userRoles.includes(DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE);
    }
}