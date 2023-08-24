import { Injectable } from "@angular/core";
import { DefaultRolesNames } from "../config/default-roles-names";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root',
  })
export class RolesService {

  constructor(private http: HttpClient) {

  }
  
  adminRole: string = DefaultRolesNames.DEFAULT_ADMIN_ROLE;
  managerRole: string = DefaultRolesNames.DEFAULT_MANAGER_ROLE;
  employeeRole: string = DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE;

  isAdmin(roles: string[]): boolean {
    return roles.includes(DefaultRolesNames.DEFAULT_ADMIN_ROLE);
  }

  isManager(roles: string[]): boolean {
    return roles.includes(DefaultRolesNames.DEFAULT_MANAGER_ROLE);
  }

  isEmployee(roles: string[]): boolean {
    return roles.includes(DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE);
  }

  getAllRoles(): Observable<string[]> {
    const url = environment.baseUrl + "api/users/roles"
    return this.http.get<string[]>(url);
  }
}