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

    getUserName() : string | null {
        return localStorage.getItem(this.userNameKey);
    }

    getRoles() : string[] {
        let roles = localStorage.getItem(this.rolesKey);
        if (roles) return JSON.parse(roles);
        else return [];
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

    passwordMatchValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            let password = control.get('password');
            let passwordConfirm = control.get('passwordConfirm');
            if (password && passwordConfirm
                && password.value !== "" && passwordConfirm.value !== ""
                && password?.value != passwordConfirm?.value) {
                return { passwordMatchError: true };
            }
            return null;
          };
    }
}