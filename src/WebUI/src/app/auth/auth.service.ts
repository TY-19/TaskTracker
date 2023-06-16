import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { LoginRequest } from "./login-request";
import { Observable, Subject, tap } from "rxjs";
import { LoginResult } from "./login-result";
import { DefaultRolesNames } from "../config/default-roles-names";
import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";
import { RegistrationRequest } from "./registration/registration-request";
import { RegistrationResult } from "./registration/registration-result";

@Injectable({
    providedIn: 'root',
})

export class AuthService {
    
    public tokenKey: string = "token";
    private _authStatus = new Subject<boolean>();
    public authStatus = this._authStatus.asObservable();
    
    private rolesKey: string = "roles";
    private userRoles: string[] = [];

    private userNameKey: string = "UserName";
    private _userName = new Subject<string|null>();
    public userName = this._userName.asObservable();

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

    setUserName(userName?: string | null) : void {
        if (!userName)
            this._userName.next(null);
        else
            this._userName.next(userName);
    }

    getRoles() : string[] {
        let roles = localStorage.getItem(this.rolesKey);
        if (roles) return JSON.parse(roles);
        else return [];
    }

    getUserName() : string | null {
        return localStorage.getItem(this.userNameKey);
    }

    registration(registrationRequest: RegistrationRequest): Observable<RegistrationResult> {
        const url = "api/Account/registration";
        return this.http.post<RegistrationResult>(url, registrationRequest);
    }

    login(loginRequest: LoginRequest): Observable<LoginResult> {
        const url = "api/Account/login";
        return this.http.post<LoginResult>(url, loginRequest)
            .pipe(tap(loginResult => {
                if(loginResult.success && loginResult.token) {
                    this.setAuthStatus(true);
                    this.setUserName(loginResult.userName);
                    this.userRoles = loginResult.roles;
                    this.writeToLocalStorage(loginResult);
                }
            }));
    }

    writeToLocalStorage(loginResult: LoginResult) {
        localStorage.setItem(this.tokenKey, loginResult.token!);
        localStorage.setItem(this.rolesKey, JSON.stringify(loginResult.roles));
        if (loginResult.userName)
            localStorage.setItem(this.userNameKey, loginResult.userName);
    }

    logout() {
        localStorage.removeItem(this.tokenKey);
        localStorage.removeItem(this.rolesKey);
        localStorage.removeItem(this.userNameKey);
        this.setAuthStatus(false);
        this.setUserName(null);
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