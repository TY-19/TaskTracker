import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { LoginRequest } from "../models/login-request";
import { Observable, Subject, tap } from "rxjs";
import { LoginResult } from "../models/login-result";
import { DefaultRolesNames } from "../config/default-roles-names";
import { RegistrationRequest } from "../models/registration-request";
import { RegistrationResult } from "../models/registration-result";
import { environment } from "src/environments/environment";

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

    private employeeIdKey: string = "EmployeeId";

    constructor(protected http: HttpClient) {
        this.init();
    }

    init(): void {
        this.setAuthStatus(this.isAuthenticated());
        this.userRoles = this.getRoles();
    }

    private setAuthStatus(isAuthenticated: boolean): void {
        this._authStatus.next(isAuthenticated);
    }

    registration(registrationRequest: RegistrationRequest): Observable<RegistrationResult> {
        const url = environment.baseUrl + "api/Account/registration";
        return this.http.post<RegistrationResult>(url, registrationRequest);
    }

    login(loginRequest: LoginRequest): Observable<LoginResult> {
        const url = environment.baseUrl + "api/Account/login";
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
    private setUserName(userName?: string | null) : void {
        if (!userName)
            this._userName.next(null);
        else
            this._userName.next(userName);
    }

    private writeToLocalStorage(loginResult: LoginResult): void {
        localStorage.setItem(this.tokenKey, loginResult.token!);
        localStorage.setItem(this.rolesKey, JSON.stringify(loginResult.roles));
        if(loginResult.userName)
            localStorage.setItem(this.userNameKey, loginResult.userName);
        if(loginResult.employeeId)
            localStorage.setItem(this.employeeIdKey, loginResult.employeeId.toString());
    }

    logout(): void {
        this.clearLocalStorage();
        this.setAuthStatus(false);
        this.setUserName(null);
        this.userRoles = [];
      }

    private clearLocalStorage(): void {
        localStorage.removeItem(this.tokenKey);
        localStorage.removeItem(this.rolesKey);
        localStorage.removeItem(this.userNameKey);
        localStorage.removeItem(this.employeeIdKey);
    }

    getToken() : string | null {
        return localStorage.getItem(this.tokenKey);
    }

    getRoles() : string[] {
        const roles = localStorage.getItem(this.rolesKey);
        if (roles) return JSON.parse(roles);
        else return [];
    }

    getUserName() : string | null {
        return localStorage.getItem(this.userNameKey);
    }

    getEmployeeId() : string | null {
        return localStorage.getItem(this.employeeIdKey);
    }

    isAuthenticated() : boolean {
        return this.getToken() != null;
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