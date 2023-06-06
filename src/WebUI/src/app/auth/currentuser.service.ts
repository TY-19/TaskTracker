import { Injectable } from "@angular/core";
import { AuthService } from "./auth.service";
import { DefaultRolesNames } from "../config/default-roles-names";

@Injectable({
    providedIn: 'root',
})

export class CurrentUserService {
    
    userRoles: string[] = [];

    constructor(private auth: AuthService) {

    }

    ngOnInit() {
        this.Subscribe();
    }

    private Subscribe() {
        this.auth.userRoles.subscribe({
            next: roles => this.userRoles = roles,
            error: error => console.log(error)
        });
    }

    isAdmin() : boolean {
        this.Subscribe();
        return this.userRoles.includes(DefaultRolesNames.DEFAULT_ADMIN_ROLE);
    }
    isManager() : boolean {
        return this.userRoles.includes(DefaultRolesNames.DEFAULT_MANAGER_ROLE);
    }
    isEmployee() : boolean {
        return this.userRoles.includes(DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE);
    }
}