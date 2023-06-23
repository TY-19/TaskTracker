import { Injectable } from "@angular/core";
import { DefaultRolesNames } from "../config/default-roles-names";

@Injectable({
    providedIn: 'root',
  })
export class RolesService {
    
  adminRole = DefaultRolesNames.DEFAULT_ADMIN_ROLE;
  managerRole = DefaultRolesNames.DEFAULT_MANAGER_ROLE;
  employeeRole = DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE;

  isAdmin(roles: string[]) : boolean {
    return roles.includes(DefaultRolesNames.DEFAULT_ADMIN_ROLE);
  }

  isManager(roles: string[]) : boolean {
    return roles.includes(DefaultRolesNames.DEFAULT_MANAGER_ROLE);
  }

  isEmployee(roles: string[]) : boolean {
    return roles.includes(DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE);
  }
}